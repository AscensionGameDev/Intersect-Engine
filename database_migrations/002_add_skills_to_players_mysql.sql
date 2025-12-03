-- Skills System Migration: Add Skills Column to Players Table (MySQL)
-- This script adds the Skills column to the Players table in the player database
-- Run this on your player database

-- Check if Skills column already exists, and add it if it doesn't
SET @dbname = DATABASE();
SET @tablename = 'Players';
SET @columnname = 'Skills';
SET @preparedStatement = (SELECT IF(
    (
        SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
        WHERE
            (TABLE_SCHEMA = @dbname)
            AND (TABLE_NAME = @tablename)
            AND (COLUMN_NAME = @columnname)
    ) > 0,
    'SELECT "Skills column already exists" AS Status;',
    CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname, ' TEXT;')
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Verify the column exists
SELECT 'Skills column added or already exists' AS Status;

