import { Request, Response } from "express";
import { getDatabase } from "../database";
import {
  PaymentRecord,
  CreatePaymentRecord,
  UpdatePaymentRecord,
  ApiResponse,
} from "../type";

export class PaymentRecordsController {
  // GET all payment records
  async getAll(
    req: Request,
    res: Response<ApiResponse<PaymentRecord[]>>,
  ): Promise<void> {
    try {
      const db = getDatabase();
      const paymentRecords = await db.all<PaymentRecord[]>(`
                SELECT pr.*, t.TransactionId as TransactionReference
                FROM PaymentRecords pr
                JOIN Transactions t ON pr.TransactionId = t.Id
                ORDER BY pr.PaymentDate DESC
            `);

      res.json({
        success: true,
        data: paymentRecords,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET payment record by ID
  async getById(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<PaymentRecord>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const paymentRecord = await db.get<PaymentRecord>(
        `SELECT pr.*, t.TransactionId as TransactionReference
                FROM PaymentRecords pr
                JOIN Transactions t ON pr.TransactionId = t.Id
                WHERE pr.Id = ?`,
        [id],
      );

      if (paymentRecord) {
        res.json({
          success: true,
          data: paymentRecord,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Payment record not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET payment records by transaction ID
  async getByTransactionId(
    req: Request<{ transactionId: string }>,
    res: Response<ApiResponse<PaymentRecord[]>>,
  ): Promise<void> {
    try {
      const { transactionId } = req.params;
      const db = getDatabase();

      const paymentRecords = await db.all<PaymentRecord[]>(
        `SELECT pr.*, t.TransactionId as TransactionReference
                FROM PaymentRecords pr
                JOIN Transactions t ON pr.TransactionId = t.Id
                WHERE pr.TransactionId = ?
                ORDER BY pr.PaymentDate DESC`,
        [transactionId],
      );

      res.json({
        success: true,
        data: paymentRecords,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // CREATE new payment record
  async create(
    req: Request<{}, {}, CreatePaymentRecord>,
    res: Response<ApiResponse<PaymentRecord>>,
  ): Promise<void> {
    try {
      const {
        TransactionId,
        PaymentMethod,
        Status,
        Amount,
        ReferenceNumber,
        PaymentDetails,
        PaymentDate,
        CompletionDate,
      } = req.body;

      const createDate = new Date().toISOString();
      const db = getDatabase();

      const result = await db.run(
        `INSERT INTO PaymentRecords 
                (TransactionId, PaymentMethod, Status, Amount,
                 ReferenceNumber, PaymentDetails, PaymentDate,
                 CompletionDate, CreateDate, UpdateDate) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
        [
          TransactionId,
          PaymentMethod,
          Status,
          Amount,
          ReferenceNumber || null,
          PaymentDetails || null,
          PaymentDate,
          CompletionDate || null,
          createDate,
          null,
        ],
      );

      const newPaymentRecord = await db.get<PaymentRecord>(
        `SELECT pr.*, t.TransactionId as TransactionReference
                FROM PaymentRecords pr
                JOIN Transactions t ON pr.TransactionId = t.Id
                WHERE pr.Id = ?`,
        [result.lastID],
      );

      res.status(201).json({
        success: true,
        data: newPaymentRecord!,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // UPDATE payment record
  async update(
    req: Request<{ id: string }, {}, UpdatePaymentRecord>,
    res: Response<ApiResponse<PaymentRecord>>,
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
        (field) => updateData[field as keyof UpdatePaymentRecord],
      );

      // Add UpdateDate
      fields.push("UpdateDate");
      values.push(new Date().toISOString());

      const query = `UPDATE PaymentRecords SET ${setClause} WHERE Id = ?`;
      values.push(id);

      const result = await db.run(query, values);

      if (result.changes && result.changes > 0) {
        const updatedPaymentRecord = await db.get<PaymentRecord>(
          `SELECT pr.*, t.TransactionId as TransactionReference
                    FROM PaymentRecords pr
                    JOIN Transactions t ON pr.TransactionId = t.Id
                    WHERE pr.Id = ?`,
          [id],
        );
        res.json({
          success: true,
          data: updatedPaymentRecord!,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Payment record not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // DELETE payment record
  async delete(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<{ message: string }>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const result = await db.run("DELETE FROM PaymentRecords WHERE Id = ?", [
        id,
      ]);

      if (result.changes && result.changes > 0) {
        res.json({
          success: true,
          message: "Payment record deleted successfully",
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Payment record not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET payment records by status
  async getByStatus(
    req: Request<{ status: string }>,
    res: Response<ApiResponse<PaymentRecord[]>>,
  ): Promise<void> {
    try {
      const { status } = req.params;
      const db = getDatabase();

      const paymentRecords = await db.all<PaymentRecord[]>(
        `SELECT pr.*, t.TransactionId as TransactionReference
                FROM PaymentRecords pr
                JOIN Transactions t ON pr.TransactionId = t.Id
                WHERE pr.Status = ?
                ORDER BY pr.PaymentDate DESC`,
        [status],
      );

      res.json({
        success: true,
        data: paymentRecords,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET payment records by date range
  async getByDateRange(
    req: Request<{}, {}, {}, { startDate: string; endDate: string }>,
    res: Response<ApiResponse<PaymentRecord[]>>,
  ): Promise<void> {
    try {
      const { startDate, endDate } = req.query;
      const db = getDatabase();

      const paymentRecords = await db.all<PaymentRecord[]>(
        `SELECT pr.*, t.TransactionId as TransactionReference
                FROM PaymentRecords pr
                JOIN Transactions t ON pr.TransactionId = t.Id
                WHERE pr.PaymentDate BETWEEN ? AND ?
                ORDER BY pr.PaymentDate DESC`,
        [startDate, endDate],
      );

      res.json({
        success: true,
        data: paymentRecords,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }
}
