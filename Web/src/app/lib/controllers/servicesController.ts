import { Request, Response } from "express";
import { getDatabase } from "../database";
import { Service, CreateService, UpdateService, ApiResponse } from "../type";

export class ServicesController {
  // GET all services
  async getAll(
    req: Request,
    res: Response<ApiResponse<Service[]>>,
  ): Promise<void> {
    try {
      const db = getDatabase();
      const services = await db.all<Service[]>(`
                SELECT s.*, sc.Type as CategoryType, sc.NameEn as CategoryName
                FROM Services s
                JOIN ServiceCategories sc ON s.ServiceCategoryId = sc.Id
                ORDER BY s.CreateDate DESC
            `);

      res.json({
        success: true,
        data: services,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET service by ID
  async getById(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<Service>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const service = await db.get<Service>(
        `SELECT s.*, sc.Type as CategoryType, sc.NameEn as CategoryName
                FROM Services s
                JOIN ServiceCategories sc ON s.ServiceCategoryId = sc.Id
                WHERE s.Id = ?`,
        [id],
      );

      if (service) {
        res.json({
          success: true,
          data: service,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET services by category
  async getByCategory(
    req: Request<{ categoryId: string }>,
    res: Response<ApiResponse<Service[]>>,
  ): Promise<void> {
    try {
      const { categoryId } = req.params;
      const db = getDatabase();

      const services = await db.all<Service[]>(
        "SELECT * FROM Services WHERE ServiceCategoryId = ? ORDER BY CreateDate DESC",
        [categoryId],
      );

      res.json({
        success: true,
        data: services,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // GET available services
  async getAvailable(
    req: Request,
    res: Response<ApiResponse<Service[]>>,
  ): Promise<void> {
    try {
      const db = getDatabase();
      const services = await db.all<Service[]>(`
                SELECT s.*, sc.Type as CategoryType
                FROM Services s
                JOIN ServiceCategories sc ON s.ServiceCategoryId = sc.Id
                WHERE s.IsAvailable = 1 AND sc.IsActive = 1
                ORDER BY sc.SortOrder, s.CreateDate
            `);

      res.json({
        success: true,
        data: services,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // CREATE new service
  async create(
    req: Request<{}, {}, CreateService>,
    res: Response<ApiResponse<Service>>,
  ): Promise<void> {
    try {
      const {
        Name,
        Type,
        Price,
        Icon,
        Color,
        IsAvailable = 1,
        NameEn,
        NameRw,
        NameFr,
        DescriptionEn,
        DescriptionRw,
        DescriptionFr,
        UpdatedBy,
        ServiceCategoryId,
      } = req.body;

      const createDate = new Date().toISOString();
      const db = getDatabase();

      const result = await db.run(
        `INSERT INTO Services 
                (Name, Type, Price, Icon, Color, IsAvailable,
                 NameEn, NameRw, NameFr, DescriptionEn, DescriptionRw, DescriptionFr,
                 CreateDate, UpdateDate, UpdatedBy, ServiceCategoryId) 
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
        [
          Name,
          Type,
          Price,
          Icon,
          Color,
          IsAvailable,
          NameEn,
          NameRw,
          NameFr,
          DescriptionEn || null,
          DescriptionRw || null,
          DescriptionFr || null,
          createDate,
          null,
          UpdatedBy || null,
          ServiceCategoryId,
        ],
      );

      const newService = await db.get<Service>(
        "SELECT * FROM Services WHERE Id = ?",
        [result.lastID],
      );

      res.status(201).json({
        success: true,
        data: newService!,
      });
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // UPDATE service
  async update(
    req: Request<{ id: string }, {}, UpdateService>,
    res: Response<ApiResponse<Service>>,
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
        (field) => updateData[field as keyof UpdateService],
      );

      // Add UpdateDate
      fields.push("UpdateDate");
      values.push(new Date().toISOString());

      const query = `UPDATE Services SET ${setClause} WHERE Id = ?`;
      values.push(id);

      const result = await db.run(query, values);

      if (result.changes && result.changes > 0) {
        const updatedService = await db.get<Service>(
          "SELECT * FROM Services WHERE Id = ?",
          [id],
        );
        res.json({
          success: true,
          data: updatedService!,
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }

  // DELETE service
  async delete(
    req: Request<{ id: string }>,
    res: Response<ApiResponse<{ message: string }>>,
  ): Promise<void> {
    try {
      const { id } = req.params;
      const db = getDatabase();

      const result = await db.run("DELETE FROM Services WHERE Id = ?", [id]);

      if (result.changes && result.changes > 0) {
        res.json({
          success: true,
          message: "Service deleted successfully",
        });
      } else {
        res.status(404).json({
          success: false,
          error: "Service not found",
        });
      }
    } catch (error) {
      res.status(500).json({
        success: false,
        error: error instanceof Error ? error.message : "Unknown error",
      });
    }
  }
}
