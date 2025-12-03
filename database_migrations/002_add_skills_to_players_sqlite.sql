-- Skills System Migration: Add Skills Column to Players Table (SQLite)
-- This script adds the Skills column to the Players table in the player database
-- Run this on: resources/playerdata.db (or your player database)

-- Note: SQLite doesn't support IF NOT EXISTS for ALTER TABLE ADD COLUMN
-- If the column already exists, this will produce an error that can be safely ignored
-- To check if the column exists first, you can run:
--   PRAGMA table_info(Players);
-- and look for a 'Skills' column in the output

-- Add Skills column to Players table
-- If you get "duplicate column name: Skills", the column already exists and you can ignore the error
ALTER TABLE Players ADD COLUMN Skills TEXT;

-- If no error occurred, the column was added successfully
SELECT 'Skills column migration completed. If you saw an error about duplicate column, it already existed.' AS Status;

