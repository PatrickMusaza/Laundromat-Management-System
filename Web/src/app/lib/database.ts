import sqlite3 from "sqlite3";
import { open, Database } from "sqlite";

let db: Database | null = null;

export async function initializeDatabase(): Promise<Database> {
  if (!db) {
    db = await open({
      filename: "./laundromat.db3",
      driver: sqlite3.Database,
    });

    // Enable foreign keys
    await db.run("PRAGMA foreign_keys = ON");

    console.log("Connected to SQLite database");
  }
  return db;
}

export function getDatabase(): Database {
  if (!db) {
    throw new Error("Database not initialized. Call initializeDatabase first.");
  }
  return db;
}

export async function closeDatabase(): Promise<void> {
  if (db) {
    await db.close();
    db = null;
    console.log("Database connection closed");
  }
}
