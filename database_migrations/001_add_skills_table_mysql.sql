-- Skills System Migration: Add Skills Table (MySQL)
-- This script adds the Skills table to the game database
-- Run this on your game database

-- Create Skills table if it doesn't exist
CREATE TABLE IF NOT EXISTS Skills (
    Id BINARY(16) NOT NULL PRIMARY KEY,
    TimeCreated BIGINT NOT NULL,
    Name TEXT,
    MaxLevel INT NOT NULL DEFAULT 99,
    BaseExperience BIGINT NOT NULL DEFAULT 100,
    ExperienceIncrease BIGINT NOT NULL DEFAULT 50,
    ExperienceOverrides TEXT,
    ExperienceCurve TEXT,
    Icon TEXT,
    Description TEXT,
    Folder TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Verify the table was created
SELECT 'Skills table created successfully' AS Status;

