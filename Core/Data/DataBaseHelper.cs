using Core.Data.Tables;
using Core.Model;
using SQLite;
using System;
using System.IO;

namespace Core.Data
{
    public static class DataBaseHelper
    {
        public static SQLiteConnection GetConnection()
        {
            // Obtiene la ruta de la base de datos SQLite para la plataforma específica
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dbPath = Path.Combine(documentsPath, "databaselocal.db3");

            // Crea y devuelve una conexión a la base de datos
            return new SQLiteConnection(dbPath);
        }
        public static void CreateTables()
        {
            using (var conn = GetConnection())
            {
                try
                {
                    conn.CreateTable<CompanyTable>(); //--
                    conn.CreateTable<ProviderTable>(); //--
                    conn.CreateTable<TransportCompanyTable>(); //--
                    conn.CreateTable<PaymentConditionTable>(); //--
                    conn.CreateTable<AssistantComercialTable>(); //--
                    conn.CreateTable<PaymentMethodTable>(); //--
                    conn.CreateTable<IncotermTable>(); //--
                    conn.CreateTable<FreightInChargeTable>(); //--
                    conn.CreateTable<CustomerTable>(); //--
                    conn.CreateTable<ProductTable>(); //--
                    conn.CreateTable<OrderNoteTable>(); //--
                    // Si tienes más tablas, agrégales aquí también.
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
