import { Request, Response } from "express";
import { getDatabase } from "../database";
import {
  ServiceCategory,
  CreateServiceCategory,
  UpdateServiceCategory,
  ApiResponse,
  PaginatedResponse,
} from "../type";

export class ServiceCategoriesController {
  // GET all service categories
  async getAll(
    req: Request,
    res: Response<ApiResponse<ServiceCategory[]>>,
  ): Promise<void> {
    try {
      const db = getDatabase();
      const categories = await db.all<ServiceCategory[]>(`
                SELECT * FROM ServiceCategories 
                ORDER BY SortOrder ASC
            `);

      res.json({
        success: true,
        data: categories,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET service category by ID
  async getById(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<ServiceCategory>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const category = await db.get<ServiceCategory>(
        "SELECT * FROM ServiceCategories WHERE Id = ?",
        [id],
      );

      if (category) {
        res.json({
          success: true,
          data: category,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service category not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET service category by Type
  async getByType(
    req: Request<{ type: string }>,
    res: Response<ApiResponse<ServiceCategory>>,
  ): Promise<void> {
    try {
      const { type } = req.params;
      const db = getDatabase();

      const category = await db.get<ServiceCategory>(
        "SELECT * FROM ServiceCategories WHERE Type = ?",
        [type],
      );

      if (category) {
        res.json({
          success: true,
          data: category,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service category not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // CREATE new service category
  async create(
    req: Request<{}, {}, CreateServiceCategory>,
    res: Response<ApiResponse<ServiceCategory>>,
  ): Promise<void> {
    try {
      const {
        Type,
        Icon,
        Color,
        NameEn,
        NameRw,
        NameFr,
        SortOrder,
        IsActive = 1,
        UpdatedBy,
      } = req.body;

      const createDate = new Date().toISOString();
      const db = getDatabase();

      const result = await db.run(
        `INSERT INTO ServiceCategories 
                (Type, Icon, Color, NameEn, NameRw, NameFr, SortOrder, 
                 IsActive, CreateDate, UpdateDate, UpdatedBy) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
        [
          Type,
          Icon || null,
          Color,
          NameEn,
          NameRw,
          NameFr,
          SortOrder,
          IsActive,
          createDate,
          null,
          UpdatedBy || null,
        ],
      );

      const newCategory = await db.get<ServiceCategory>(
        "SELECT * FROM ServiceCategories WHERE Id = ?",
        [result.lastID],
      );

      res.status(201).json({
        success: true,
        data: newCategory!,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // UPDATE service category
  async update(
    req: Request<{ id: string }, {}, UpdateServiceCategory>,
    res: Response<ApiResponse<ServiceCategory>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const updateData = req.body;
      const db = getDatabase();

      // Build dynamic update query
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
        (field) => updateData[field as keyof UpdateServiceCategory],
      );

      // Add UpdateDate
      fields.push("UpdateDate");
      values.push(new Date().toISOString());

      const query = `UPDATE ServiceCategories SET ${setClause} WHERE Id = ?`;
      values.push(id);

      const result = await db.run(query, values);

      if (result.changes && result.changes > 0) {
        const updatedCategory = await db.get<ServiceCategory>(
          "SELECT * FROM ServiceCategories WHERE Id = ?",
          [id],
        );
        res.json({
          success: true,
          data: updatedCategory!,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service category not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // DELETE service category
  async delete(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<{ message: string }>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const result = await db.run(
        "DELETE FROM ServiceCategories WHERE Id = ?",
        [id],
      );

      if (result.changes && result.changes > 0) {
        res.json({
          success: true,
          message: "Service category deleted successfully",
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service category not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET active service categories
  async getActive(
    req: Request,
    res: Response<ApiResponse<ServiceCategory[]>>,
  ): Promise<void> {
    try {
      const db = getDatabase();
      const categories = await db.all<ServiceCategory[]>(`
                SELECT * FROM ServiceCategories 
                WHERE IsActive = 1 
                ORDER BY SortOrder ASC
            `);

      res.json({
        success: true,
        data: categories,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }
}
