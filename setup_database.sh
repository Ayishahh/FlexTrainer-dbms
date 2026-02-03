#!/bin/bash

# Flex Trainer Database Setup Script
# This script initializes the SQL Server database in Docker

echo "Starting SQL Server container..."
docker-compose up -d

echo "Waiting for SQL Server to be ready (30 seconds)..."
sleep 30

# Check if SQL Server is responding
echo "Checking SQL Server connection..."
docker exec flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -Q "SELECT 1" > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "SQL Server is ready!"
    
    echo "Creating database and tables..."
    docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/01_schema.sql
    
    echo "Creating triggers..."
    docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/02_triggers.sql
    
    echo "Creating stored procedures..."
    docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/03_procedures.sql
    
    echo "Inserting sample data..."
    docker exec -i flex-trainer-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FlexTrainer2024! -C -i /docker-entrypoint-initdb.d/04_sample_data.sql
    
    echo ""
    echo "============================================"
    echo "Database setup complete!"
    echo "============================================"
    echo ""
    echo "Connection details for C# application:"
    echo "  Server: localhost,1433"
    echo "  Database: DB_PROJECT"
    echo "  Username: sa"
    echo "  Password: FlexTrainer2024!"
    echo ""
    echo "Connection string:"
    echo 'Data Source=localhost,1433;Initial Catalog=DB_PROJECT;User ID=sa;Password=FlexTrainer2024!;TrustServerCertificate=True'
    echo ""
else
    echo "ERROR: SQL Server is not responding. Please check Docker logs:"
    echo "docker logs flex-trainer-db"
fi
