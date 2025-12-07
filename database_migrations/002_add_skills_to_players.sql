-- Skills System Migration: Add Skills Column to Players Table (SQLite)
-- Run this on: resources/playerdata.db
-- 
-- This script adds the Skills column to the Players table to store player skill progress.
-- The Skills column stores JSON serialized Dictionary<Guid, SkillData>

-- Note: SQLite doesn't support IF NOT EXISTS for ALTER TABLE ADD COLUMN
-- If the column already exists, you'll get an error: "duplicate column name: Skills"
-- This error is safe to ignore - it means the migration was already run

-- Check current columns (for reference)
-- Uncomment the next line to see all columns before adding:
-- PRAGMA table_info(Players);

-- Add Skills column to Players table
ALTER TABLE Players ADD COLUMN Skills TEXT;

-- If you see "duplicate column name: Skills" above, the column already exists
-- This is normal if you've run this migration before

-- Verify the column was added
-- Uncomment to check:
-- PRAGMA table_info(Players);

SELECT 'Skills column migration completed. If you saw a duplicate column error, it already existed.' AS Status;

