-- Skills System Migration: Add Skills Table (SQLite)
-- This script adds the Skills table to the game database
-- Run this on: resources/gamedata.db (or your game database)

-- Create Skills table if it doesn't exist
CREATE TABLE IF NOT EXISTS Skills (
    Id BLOB NOT NULL PRIMARY KEY,
    TimeCreated INTEGER NOT NULL,
    Name TEXT,
    MaxLevel INTEGER NOT NULL DEFAULT 99,
    BaseExperience INTEGER NOT NULL DEFAULT 100,
    ExperienceIncrease INTEGER NOT NULL DEFAULT 50,
    ExperienceOverrides TEXT,
    ExperienceCurve TEXT,
    Icon TEXT,
    Description TEXT,
    Folder TEXT
);

-- Verify the table was created
SELECT 'Skills table created successfully' AS Status;

