import { Request, Response } from "express";
import { getDatabase } from "../database";
import {
  Transaction,
  CreateTransaction,
  UpdateTransaction,
  TransactionItem,
  CreateTransactionItem,
  ApiResponse,
} from "../types";

export class TransactionsController {
  // GET all transactions
  async getAll(
    req: Request,
    res: Response<ApiResponse<Transaction[]>>,
  ): Promise<void> {
    try {
      const db = getDatabase();
      const transactions = await db.all<Transaction[]>(`
                SELECT * FROM Transactions 
                ORDER BY TransactionDate DESC, CreateDate DESC
            `);

      res.json({
        success: true,
        data: transactions,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET transaction by ID
  async getById(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<Transaction & { items: TransactionItem[] }>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const transaction = await db.get<Transaction>(
        "SELECT * FROM Transactions WHERE Id = ?",
        [id],
      );

      if (transaction) {
        const items = await db.all<TransactionItem[]>(
          "SELECT * FROM TransactionItems WHERE TransactionId = ?",
          [id],
        );

        res.json({
          success: true,
          data: { ...transaction, items },
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Transaction not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET transaction by TransactionId (unique identifier)
  async getByTransactionId(
    req: Request<{ transactionId: string }>,
    res: Response<ApiResponse<Transaction & { items: TransactionItem[] }>>,
  ): Promise<void> {
    try {
      const { transactionId } = req.params;
      const db = getDatabase();

      const transaction = await db.get<Transaction>(
        "SELECT * FROM Transactions WHERE TransactionId = ?",
        [transactionId],
      );

      if (transaction) {
        const items = await db.all<TransactionItem[]>(
          "SELECT * FROM TransactionItems WHERE TransactionId = ?",
          [transaction.Id],
        );

        res.json({
          success: true,
          data: { ...transaction, items },
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Transaction not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // CREATE new transaction with items
  async create(
    req: Request<
      {},
      {},
      CreateTransaction & { items: CreateTransactionItem[] }
    >,
    res: Response<ApiResponse<Transaction & { items: TransactionItem[] }>>,
  ): Promise<void> {
    try {
      const {
        TransactionId,
        Status,
        PaymentMethod,
        Subtotal,
        TaxAmount,
        TotalAmount,
        CashReceived,
        ChangeAmount,
        CustomerName,
        CustomerTin,
        CustomerPhone,
        TransactionDate,
        PaymentDate,
        CompletionDate,
        UpdatedBy,
        items,
      } = req.body;

      const createDate = new Date().toISOString();
      const db = getDatabase();

      // Start transaction
      await db.run("BEGIN TRANSACTION");

      try {
        // Insert transaction
        const transactionResult = await db.run(
          `INSERT INTO Transactions 
                    (TransactionId, Status, PaymentMethod, Subtotal, TaxAmount, TotalAmount,
                     CashReceived, ChangeAmount, CustomerName, CustomerTin, CustomerPhone,
                     TransactionDate, PaymentDate, CompletionDate, CreateDate, UpdateDate, UpdatedBy) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
          [
            TransactionId,
            Status,
            PaymentMethod,
            Subtotal,
            TaxAmount,
            TotalAmount,
            CashReceived || null,
            ChangeAmount || null,
            CustomerName,
            CustomerTin,
            CustomerPhone,
            TransactionDate,
            PaymentDate || null,
            CompletionDate || null,
            createDate,
            null,
            UpdatedBy || null,
          ],
        );

        const transactionId = transactionResult.lastID;

        // Insert transaction items
        const transactionItems: TransactionItem[] = [];
        for (const item of items) {
          const itemResult = await db.run(
            `INSERT INTO TransactionItems 
                        (TransactionId, ServiceId, ServiceName, ServiceDescription,
                         UnitPrice, Quantity, TotalPrice, ServiceType, ServiceIcon, CreateDate)
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
            [
              transactionId,
              item.ServiceId,
              item.ServiceName,
              item.ServiceDescription || null,
              item.UnitPrice,
              item.Quantity,
              item.TotalPrice,
              item.ServiceType,
              item.ServiceIcon,
              createDate,
            ],
          );

          const newItem = await db.get<TransactionItem>(
            "SELECT * FROM TransactionItems WHERE Id = ?",
            [itemResult.lastID],
          );
          transactionItems.push(newItem!);
        }

        // Commit transaction
        await db.run("COMMIT");

        const newTransaction = await db.get<Transaction>(
          "SELECT * FROM Transactions WHERE Id = ?",
          [transactionId],
        );

        res.status(201).json({
          success: true,
          data: { ...newTransaction!, items: transactionItems },
        });
      } catch (error) {
        await db.run("ROLLBACK");
        throw error;
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // UPDATE transaction
  async update(
    req: Request<{ id: string }, {}, UpdateTransaction>,
    res: Response<ApiResponse<Transaction>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const updateData = req.body;
      const db = getDatabase();

      const fields = Object.keys(updateData);
      if (fields.length === 0) {
        res.status(400).json({
          success: false,
          error: "No fields to update",
        });
        return;
      }

      const setClause = fields.map((field) => `${field} = ?`).join(", ");
      const values = fields.map(
        (field) => updateData[field as keyof UpdateTransaction],
      );

      // Add UpdateDate
      fields.push("UpdateDate");
      values.push(new Date().toISOString());

      const query = `UPDATE Transactions SET ${setClause} WHERE Id = ?`;
      values.push(id);

      const result = await db.run(query, values);

      if (result.changes && result.changes > 0) {
        const updatedTransaction = await db.get<Transaction>(
          "SELECT * FROM Transactions WHERE Id = ?",
          [id],
        );
        res.json({
          success: true,
          data: updatedTransaction!,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Transaction not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // DELETE transaction
  async delete(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<{ message: string }>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const result = await db.run("DELETE FROM Transactions WHERE Id = ?", [
        id,
      ]);

      if (result.changes && result.changes > 0) {
        res.json({
          success: true,
          message: "Transaction deleted successfully",
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Transaction not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET transactions by date range
  async getByDateRange(
    req: Request<{}, {}, {}, { startDate: string; endDate: string }>,
    res: Response<ApiResponse<Transaction[]>>,
  ): Promise<void> {
    try {
      const { startDate, endDate } = req.query;
      const db = getDatabase();

      const transactions = await db.all<Transaction[]>(
        `SELECT * FROM Transactions 
                WHERE TransactionDate BETWEEN ? AND ?
                ORDER BY TransactionDate DESC`,
        [startDate, endDate],
      );

      res.json({
        success: true,
        data: transactions,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET transactions by status
  async getByStatus(
    req: Request<{ status: string }>,
    res: Response<ApiResponse<Transaction[]>>,
  ): Promise<void> {
    try {
      const { status } = req.params;
      const db = getDatabase();

      const transactions = await db.all<Transaction[]>(
        "SELECT * FROM Transactions WHERE Status = ? ORDER BY TransactionDate DESC",
        [status],
      );

      res.json({
        success: true,
        data: transactions,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }
}
