// server.js
import express from "express";
import cors from "cors";

import sqlite3 from "sqlite3";
import { open } from "sqlite";

import path from "path";
import { fileURLToPath } from "url";

import dotenv from "dotenv";
dotenv.config();

// ===============================
// ESM __dirname replacement
// ===============================
const __filename = fileURLToPath(import.meta.url);

// ===============================
// App setup
// ===============================
const app = express();
const PORT = 3001;

app.use(cors());
app.use(express.json());

let db;

// ===============================
// Database initialization
// ===============================
async function initializeDatabase() {
  const dbPath = process.env.LAUNDROMAT_DB_PATH;

  if (!dbPath) {
    throw new Error("LAUNDROMAT_DB_PATH is not set");
  }

  db = await open({
    filename: dbPath,
    driver: sqlite3.Database,
  });

  console.log("✅ Connected to SQLite database:", dbPath);

  await db.run("PRAGMA foreign_keys = ON");
  return db;
}

// ===============================
// ROUTES
// ===============================

// Health check
app.get("/health", (req, res) => {
  res.json({ status: "OK", timestamp: new Date().toISOString() });
});

app.get("/api/health", (req, res) => {
  res.json({ status: "OK", timestamp: new Date().toISOString() });
});

// ===============================
// SERVICE CATEGORIES
// ===============================
app.get("/service-categories", async (req, res) => {
  try {
    const categories = await db.all(
      "SELECT * FROM ServiceCategories ORDER BY SortOrder ASC"
    );
    res.json({ success: true, data: categories });
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.get("/api/service-categories", async (req, res) => {
  try {
    const categories = await db.all(
      "SELECT * FROM ServiceCategories ORDER BY SortOrder ASC"
    );
    res.json({ success: true, data: categories });
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ===============================
// SERVICES
// ===============================
app.get("/services", async (req, res) => {
  try {
    const services = await db.all(`
      SELECT s.*, sc.Type AS CategoryType, sc.NameEn AS CategoryName
      FROM Services s
      JOIN ServiceCategories sc ON s.ServiceCategoryId = sc.Id
      ORDER BY s.CreateDate DESC
    `);
    res.json({ success: true, data: services });
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.get("/api/services", async (req, res) => {
  try {
    const services = await db.all(`
      SELECT s.*, sc.Type AS CategoryType, sc.NameEn AS CategoryName
      FROM Services s
      JOIN ServiceCategories sc ON s.ServiceCategoryId = sc.Id
      ORDER BY s.CreateDate DESC
    `);
    res.json({ success: true, data: services });
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ===============================
// TRANSACTIONS
// ===============================
app.get("/transactions", async (req, res) => {
  try {
    const transactions = await db.all(
      "SELECT * FROM Transactions ORDER BY TransactionDate DESC"
    );
    res.json({ success: true, data: transactions });
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.get("/api/transactions", async (req, res) => {
  try {
    const transactions = await db.all(
      "SELECT * FROM Transactions ORDER BY TransactionDate DESC"
    );
    res.json({ success: true, data: transactions });
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.post("/transactions", async (req, res) => {
  try {
    const createDate = new Date().toISOString();

    await db.run("BEGIN TRANSACTION");

    try {
      const result = await db.run(
        `
        INSERT INTO Transactions
        (TransactionId, Status, PaymentMethod, Subtotal, TaxAmount, TotalAmount,
         CashReceived, ChangeAmount, CustomerName, CustomerTin, CustomerPhone,
         TransactionDate, CreateDate)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `,
        [
          req.body.TransactionId || "TXN-" + Date.now(),
          req.body.Status || "pending",
          req.body.PaymentMethod || "cash",
          req.body.Subtotal || 0,
          req.body.TaxAmount || 0,
          req.body.TotalAmount || 0,
          req.body.CashReceived || null,
          req.body.ChangeAmount || null,
          req.body.CustomerName || "Guest",
          req.body.CustomerTin || "",
          req.body.CustomerPhone || "",
          req.body.TransactionDate || createDate,
          createDate,
        ]
      );

      await db.run("COMMIT");

      const newTransaction = await db.get(
        "SELECT * FROM Transactions WHERE Id = ?",
        [result.lastID]
      );

      res.status(201).json({ success: true, data: newTransaction });
    } catch (error) {
      await db.run("ROLLBACK");
      throw error;
    }
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

app.post("/api/transactions", async (req, res) => {
  try {
    const createDate = new Date().toISOString();

    await db.run("BEGIN TRANSACTION");

    try {
      const result = await db.run(
        `
        INSERT INTO Transactions
        (TransactionId, Status, PaymentMethod, Subtotal, TaxAmount, TotalAmount,
         CashReceived, ChangeAmount, CustomerName, CustomerTin, CustomerPhone,
         TransactionDate, CreateDate)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        `,
        [
          req.body.TransactionId || "TXN-" + Date.now(),
          req.body.Status || "pending",
          req.body.PaymentMethod || "cash",
          req.body.Subtotal || 0,
          req.body.TaxAmount || 0,
          req.body.TotalAmount || 0,
          req.body.CashReceived || null,
          req.body.ChangeAmount || null,
          req.body.CustomerName || "Guest",
          req.body.CustomerTin || "",
          req.body.CustomerPhone || "",
          req.body.TransactionDate || createDate,
          createDate,
        ]
      );

      await db.run("COMMIT");

      const newTransaction = await db.get(
        "SELECT * FROM Transactions WHERE Id = ?",
        [result.lastID]
      );

      res.status(201).json({ success: true, data: newTransaction });
    } catch (error) {
      await db.run("ROLLBACK");
      throw error;
    }
  } catch (error) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ===============================
// START SERVER
// ===============================
async function startServer() {
  try {
    await initializeDatabase();

    app.listen(PORT, () => {
      console.log(`
🚀 API Server Running
=======================================
📡 Direct API:      http://localhost:${PORT}
🔗 Via Proxy:       http://localhost:5173/api

✅ Health:
   http://localhost:${PORT}/health

📋 Service Categories:
   http://localhost:${PORT}/service-categories

🧺 Services:
   http://localhost:${PORT}/services

💳 Transactions:
   http://localhost:${PORT}/transactions

⚡ React App:
   http://localhost:5173
=======================================
`);
    });
  } catch (error) {
    console.error("❌ Failed to start server:", error);
    process.exit(1);
  }
}

startServer();
