# Skills System Database Migrations

This directory contains SQL scripts to add the Skills system to your Intersect database.

## Overview

The Skills system requires two database changes:
1. **Game Database** (`gamedata.db` or MySQL equivalent): Add a `Skills` table to store skill definitions
2. **Player Database** (`playerdata.db` or MySQL equivalent): Add a `Skills` column to the `Players` table to store player skill progress

## Important Notes

- **Backup your databases** before running these scripts!
- These scripts are idempotent - they can be run multiple times safely (they check if tables/columns already exist)
- Make sure your server is **stopped** before running these migrations

## Migration Steps

### For SQLite (Default)

1. **Stop your Intersect server**
2. **Backup your databases:**
   - Copy `resources/gamedata.db` to `resources/gamedata.db.backup`
   - Copy `resources/playerdata.db` to `resources/playerdata.db.backup`

3. **Run the game database migration:**
   ```bash
   sqlite3 resources/gamedata.db < database_migrations/001_add_skills_table_sqlite.sql
   ```

4. **Run the player database migration:**
   ```bash
   sqlite3 resources/playerdata.db < database_migrations/002_add_skills_to_players_sqlite.sql
   ```

5. **Start your server** and verify everything works

### For MySQL

1. **Stop your Intersect server**
2. **Backup your databases** (use your preferred MySQL backup method)

3. **Run the game database migration:**
   ```bash
   mysql -u your_username -p your_game_database < database_migrations/001_add_skills_table_mysql.sql
   ```

4. **Run the player database migration:**
   ```bash
   mysql -u your_username -p your_player_database < database_migrations/002_add_skills_to_players_mysql.sql
   ```

5. **Start your server** and verify everything works

## Verification

After running the migrations, you can verify they worked:

### SQLite
```bash
# Check Skills table exists
sqlite3 resources/gamedata.db ".tables" | grep -i skills

# Check Players table has Skills column
sqlite3 resources/playerdata.db "PRAGMA table_info(Players);" | grep -i skills
```

### MySQL
```sql
-- Check Skills table exists
SHOW TABLES LIKE 'Skills';

-- Check Players table has Skills column
DESCRIBE Players;
-- Look for a 'Skills' column in the output
```

## Troubleshooting

### "Table already exists" or "Column already exists"
This is normal if you've already run the migration. The scripts are designed to be safe to run multiple times.

### "Database is locked"
- Make sure your Intersect server is completely stopped
- Close any database management tools (like DB Browser for SQLite)
- Wait a few seconds and try again

### Migration fails partway through
1. Restore your database backups
2. Check the error message
3. Ensure your database user has proper permissions (for MySQL)
4. Try running the migration again

## Rollback

If you need to rollback these changes:

### SQLite
```sql
-- Remove Skills column from Players table
ALTER TABLE Players DROP COLUMN Skills;

-- Remove Skills table
DROP TABLE IF EXISTS Skills;
```

### MySQL
```sql
-- Remove Skills column from Players table
ALTER TABLE Players DROP COLUMN Skills;

-- Remove Skills table
DROP TABLE IF EXISTS Skills;
```

**Warning:** Rolling back will delete all skill data! Make sure you have backups.

## Support

If you encounter issues with these migrations, please:
1. Check that you've backed up your databases
2. Verify your database type (SQLite vs MySQL)
3. Check the error messages carefully
4. Ensure your server is stopped before running migrations

