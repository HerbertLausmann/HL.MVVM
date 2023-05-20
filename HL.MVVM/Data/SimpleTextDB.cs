using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL.MVVM;

namespace HL.MVVM.Data
{
    /// <summary>
    /// Classe base abstrata SimpleTextDB para armazenamento de dados em arquivos de texto simples.
    /// </summary>
    public abstract class SimpleTextDB : DisposableBase
    {
        private string _dbFilePath = null;
        private List<string> _DataBase;

        private string _dbName;
        /// <summary>
        /// Obtém o nome da base de dados.
        /// </summary>
        public string Name => _dbName;

        /// <summary>
        /// Obtém a lista de strings que representa a base de dados.
        /// </summary>
        protected List<string> DataBase
        {
            get => _DataBase;
        }

        /// <summary>
        /// Obtém um valor que indica se a base de dados foi carregada.
        /// </summary>
        public bool IsLoaded => IsLoaded;

        /// <summary>
        /// Inicializa a base de dados com o nome especificado.
        /// </summary>
        /// <param name="dbName">Nome da base de dados.</param>
        protected void Init(string dbName)
        {
            _DataBase = new List<string>();
            _dbName = dbName;
            _dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database", _dbName + ".txt");
            EnsureLoad();
        }

        private bool _WasLoaded = false;

        /// <summary>
        /// Carrega a base de dados se ainda não estiver carregada.
        /// </summary>
        public void EnsureLoad()
        {
            lock (this)
            {
                if (!_WasLoaded)
                {
                    EnsureFolderExists();
                    if (!File.Exists(_dbFilePath))
                    {
                        _WasLoaded = true;
                        return;
                    }
                    using (StreamReader reader = new StreamReader(_dbFilePath, Encoding.UTF8))
                    {
                        while (!reader.EndOfStream)
                        {
                            var r = reader.ReadLine();
                            if (!string.IsNullOrWhiteSpace(r))
                                DataBase.Add(r);
                        }
                        _WasLoaded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Método protegido para descartar a instância e salvar os dados no arquivo.
        /// </summary>
        protected override void DoDispose()
        {
            if (_WasLoaded)
            {
                EnsureFolderExists();
                using (StreamWriter w = new StreamWriter(_dbFilePath, false, System.Text.Encoding.UTF8))
                {
                    foreach (var item in DataBase)
                    {
                        w.WriteLine(item);
                        w.Flush();
                    }
                }
                DataBase.Clear();
            }
        }

        /// <summary>
        /// Garante que a pasta que contém a base de dados exista.
        /// </summary>
        private void EnsureFolderExists()
        {
            var folder = Path.GetDirectoryName(_dbFilePath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }
    }
}

