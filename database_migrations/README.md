# Skills System Database Migrations (SQLite Only)

This guide provides instructions for adding the Skills system to your Intersect SQLite databases.

## Overview

The Skills system requires two database changes:
1. **Game Database** (`resources/gamedata.db`): Add a `Skills` table to store skill definitions
2. **Player Database** (`resources/playerdata.db`): Add a `Skills` column to the `Players` table to store player skill progress

## ⚠️ IMPORTANT: Before You Begin

- **STOP YOUR INTERSECT SERVER** before making any database changes
- **BACKUP YOUR DATABASES** - Copy both database files to a safe location
- Close any database management tools (DB Browser for SQLite, etc.)
- These migrations are safe to run multiple times (idempotent)

## Method 1: Using SQL Scripts (Recommended)

### Step 1: Backup Your Databases

```bash
# Windows PowerShell
Copy-Item resources\gamedata.db resources\gamedata.db.backup
Copy-Item resources\playerdata.db resources\playerdata.db.backup

# Linux/Mac
cp resources/gamedata.db resources/gamedata.db.backup
cp resources/playerdata.db resources/playerdata.db.backup
```

### Step 2: Run the Migration Scripts

**For Game Database (gamedata.db):**
```bash
sqlite3 resources/gamedata.db < database_migrations/001_add_skills_table.sql
```

**For Player Database (playerdata.db):**
```bash
sqlite3 resources/playerdata.db < database_migrations/002_add_skills_to_players.sql
```

### Step 3: Verify the Migrations

**Check Skills table exists:**
```bash
sqlite3 resources/gamedata.db ".tables" | findstr /i skills
# Should output: Skills
```

**Check Players table has Skills column:**
```bash
sqlite3 resources/playerdata.db "PRAGMA table_info(Players);" | findstr /i skills
# Should show a row with column name "Skills"
```

## Method 2: Manual SQL Commands

If you prefer to run the SQL commands manually (using DB Browser for SQLite, sqlite3 command line, etc.):

### Step 1: Backup Your Databases
Same as Method 1, Step 1.

### Step 2: Add Skills Table to Game Database

Open `resources/gamedata.db` and run:

```sql
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
```

### Step 3: Add Skills Column to Players Table

Open `resources/playerdata.db` and run:

```sql
-- Check if column already exists first
-- Run this to see all columns:
PRAGMA table_info(Players);

-- If Skills column is NOT in the list, run:
ALTER TABLE Players ADD COLUMN Skills TEXT;
```

**Note:** If you get an error "duplicate column name: Skills", the column already exists and you can ignore the error.

### Step 4: Verify the Changes

**In Game Database:**
```sql
-- Should return 1 row showing the Skills table
SELECT name FROM sqlite_master WHERE type='table' AND name='Skills';
```

**In Player Database:**
```sql
-- Should show Skills in the column list
PRAGMA table_info(Players);
```

## Verification Checklist

After running migrations, verify:

- [ ] `resources/gamedata.db` has a `Skills` table
- [ ] `resources/playerdata.db` `Players` table has a `Skills` column (TEXT type)
- [ ] Server starts without database errors
- [ ] You can create skills in the editor
- [ ] Skills appear in the client skills window

## Troubleshooting

### "Database is locked"
- Make sure the Intersect server is completely stopped
- Close all database management tools
- Wait a few seconds and try again

### "Table already exists" or "Column already exists"
- This is normal if you've already run the migration
- The scripts use `CREATE TABLE IF NOT EXISTS` and check for existing columns
- You can safely ignore these messages

### "No such table: Skills" after migration
- Verify you ran the migration on the correct database file
- Check that the migration script completed without errors
- Try running the migration again

### "No such column: p.Skills" after migration
- Verify you ran the player database migration
- Check that the `Skills` column exists: `PRAGMA table_info(Players);`
- If missing, run the migration again

## Rollback (If Needed)

If you need to remove the Skills system:

**Remove Skills column from Players:**
```sql
-- Run in playerdata.db
ALTER TABLE Players DROP COLUMN Skills;
```

**Remove Skills table:**
```sql
-- Run in gamedata.db
DROP TABLE IF EXISTS Skills;
```

**⚠️ WARNING:** This will delete all skill data! Make sure you have backups.

## Schema Reference

### Skills Table Structure
- `Id` (BLOB, PRIMARY KEY) - GUID of the skill
- `TimeCreated` (INTEGER) - Timestamp when skill was created
- `Name` (TEXT) - Name of the skill
- `MaxLevel` (INTEGER, DEFAULT 99) - Maximum level for this skill
- `BaseExperience` (INTEGER, DEFAULT 100) - Base experience required
- `ExperienceIncrease` (INTEGER, DEFAULT 50) - Experience increase per level
- `ExperienceOverrides` (TEXT) - JSON dictionary of level->experience overrides
- `ExperienceCurve` (TEXT) - JSON serialized ExperienceCurve object
- `Icon` (TEXT) - Icon texture filename
- `Description` (TEXT) - Skill description
- `Folder` (TEXT) - Folder organization path

### Players.Skills Column
- `Skills` (TEXT) - JSON serialized Dictionary<Guid, SkillData>
  - Format: `{"skill-guid-1": {"Experience": 100, "Level": 5}, "skill-guid-2": {...}}`

## Support

If you encounter issues:
1. Verify you backed up your databases
2. Check that the server is stopped
3. Review error messages carefully
4. Ensure you're modifying the correct database files
5. Try running the migrations again

