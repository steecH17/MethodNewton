using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace MethodNewton.WinForms;

public partial class Form1 : Form
{
    private string _excelFilePath;
    private List<List<double>>? xData = null;
    private List<double>? yData = null;
    private int countArgs;
    private int countDataInTable;
    private bool canEditTable;
    private DataGridView _dataGridView;
    private DataTable _dataTable;
    private AppSettings _appSettings;
    private FunctionModel? _functionModel = null;
    private StatisticModels? _statisticModels = null;
    private Label functionValueLabel;
    private Label determValueLabel;
    private Label iterationsValueLabel;
    private Label componentsValueLabel;
    private Button chartButton;
    public Form1()
    {
        InitializeComponent();
        InitializeToolStrip();
        _appSettings = new AppSettings();
        countDataInTable = 0;
        canEditTable = false;

        //xData = new List<List<double>>();
        //yData = new List<double>();
    }

    private void InitializeToolStrip()
    {
        this.Size = new Size(1300, 750);
        this.Text = "Автоматизация выбора трансцендентной функции";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9F);

        // Создаем меню
        var menuStrip = new MenuStrip { BackColor = Color.WhiteSmoke };

        var fileMenu = new ToolStripMenuItem("Файл");
        fileMenu.DropDownItems.Add("Новый", null, (s, e) => ClearData());
        fileMenu.DropDownItems.Add("Открыть...", null, (s, e) => LoadFromFile());
        fileMenu.DropDownItems.Add("Сохранить", null, (s, e) => SaveToFile());
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Настройки...", null, (s, e) => ShowSettingsForm());
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Выход", null, (s, e) => this.Close());

        var editMenu = new ToolStripMenuItem("Правка");
        editMenu.DropDownItems.Add("Добавить строку", null, (s, e) => AddEmptyRow());
        editMenu.DropDownItems.Add("Удалить строку", null, (s, e) => DeleteSelectedRows());
        editMenu.DropDownItems.Add(new ToolStripSeparator());
        editMenu.DropDownItems.Add("Копировать", null, (s, e) => CopySelectedCells());
        editMenu.DropDownItems.Add("Вставить", null, (s, e) => PasteToCells());

        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu });

        // Панель инструментов
        var toolStrip = new ToolStrip();
        toolStrip.Items.Add(new ToolStripButton("➕", null, (s, e) => AddEmptyRow()) { ToolTipText = "Добавить строку" });
        toolStrip.Items.Add(new ToolStripButton("❌", null, (s, e) => DeleteSelectedRows()) { ToolTipText = "Удалить строку" });
        toolStrip.Items.Add(new ToolStripSeparator());
        toolStrip.Items.Add(new ToolStripButton("📋", null, (s, e) => CopySelectedCells()) { ToolTipText = "Копировать" });
        toolStrip.Items.Add(new ToolStripButton("📝", null, (s, e) => PasteToCells()) { ToolTipText = "Вставить" });
        toolStrip.Items.Add(new ToolStripSeparator());
        toolStrip.Items.Add(new ToolStripButton("💾", null, (s, e) => SaveToFile()) { ToolTipText = "Сохранить" });
        toolStrip.Items.Add(new ToolStripButton("📊", null, (s, e) => ShowStatistics()) { ToolTipText = "Статистика" });

        // DataGridView
        _dataGridView = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = true,
            RowHeadersVisible = true,
            MultiSelect = true,
            SelectionMode = DataGridViewSelectionMode.RowHeaderSelect,
            EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2,
            ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        };

        // Контекстное меню
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Добавить строку", null, (s, e) => AddEmptyRow());
        contextMenu.Items.Add("Удалить строку", null, (s, e) => DeleteSelectedRows());
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Копировать", null, (s, e) => CopySelectedCells());
        contextMenu.Items.Add("Вставить", null, (s, e) => PasteToCells());
        contextMenu.Items.Add("Очистить ячейки", null, (s, e) => ClearSelectedCells());

        _dataGridView.ContextMenuStrip = contextMenu;

        // Панель статуса
        var statusStrip = new StatusStrip();
        var statusLabel = new ToolStripStatusLabel("Готово");
        var rowCountLabel = new ToolStripStatusLabel();
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, rowCountLabel });

        // Панель с кнопкой расчета и результатами
        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 140,
            BackColor = Color.FromArgb(240, 240, 240),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Кнопка "Найти функцию"
        var calculateButton = new Button
        {
            Text = "🔍 НАЙТИ ФУНКЦИЮ",
            Location = new Point(10, 15),
            Size = new Size(150, 45),
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat
        };
        calculateButton.FlatAppearance.BorderSize = 0;
        calculateButton.Click += CalculateButton_Click;

        // Группа с результатами
        var resultGroupBox = new GroupBox
        {
            Text = "Результаты аппроксимации",
            Location = new Point(170, 5),
            Size = new Size(1100, 120),
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        // Первая строка
        var functionLabel = new Label
        {
            Text = "Найденная функция:",
            Location = new Point(10, 20),
            Size = new Size(170, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft
        };

        functionValueLabel = new Label
        {
            Text = "—",
            Location = new Point(200, 20),
            Size = new Size(550, 25),
            Font = new Font("Consolas", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 102, 204),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        var r2Label = new Label
        {
            Text = "R² =",
            Location = new Point(10, 80),
            Size = new Size(50, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };

        determValueLabel = new Label
        {
            Text = "—",
            Location = new Point(70, 80),
            Size = new Size(80, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.Green,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var iterationsLabel = new Label
        {
            Text = "Итераций:",
            Location = new Point(190, 80),
            Size = new Size(120, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };

        iterationsValueLabel = new Label
        {
            Text = "—",
            Location = new Point(330, 80),
            Size = new Size(50, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Вторая строка
        var componentsLabel = new Label
        {
            Text = "Компонентов:",
            Location = new Point(10, 50),
            Size = new Size(120, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        componentsValueLabel = new Label
        {
            Text = "—",
            Location = new Point(150, 50),
            Size = new Size(50, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Кнопка для построения графика
        chartButton = new Button
        {
            Text = "📈 Построить график",
            Location = new Point(850, 45),
            Size = new Size(160, 35),
            Enabled = false,
            FlatStyle = FlatStyle.Standard,
            BackColor = SystemColors.Control,
            UseVisualStyleBackColor = true,
            Font = new Font("Segoe UI", 9)
        };
        chartButton.Click += ChartButton_Click;

        // Добавляем метки в GroupBox
        resultGroupBox.Controls.AddRange(new Control[]
        {
        functionLabel, functionValueLabel,
        r2Label, determValueLabel,
        iterationsLabel, iterationsValueLabel,
        componentsLabel, componentsValueLabel,
        chartButton
        });

        // Добавляем кнопку и GroupBox в панель
        bottomPanel.Controls.Add(calculateButton);
        bottomPanel.Controls.Add(resultGroupBox);

        // Устанавливаем Dock для всех элементов
        menuStrip.Dock = DockStyle.Top;
        toolStrip.Dock = DockStyle.Top;
        _dataGridView.Dock = DockStyle.Fill;
        bottomPanel.Dock = DockStyle.Bottom;
        statusStrip.Dock = DockStyle.Bottom;

        // ОЧЕНЬ ВАЖНО: правильный порядок добавления (сверху вниз)
        this.Controls.Clear();
        this.Controls.Add(menuStrip);        // 1. Меню - самый верх
        this.Controls.Add(toolStrip);        // 2. Панель инструментов - под меню
        this.Controls.Add(_dataGridView);    // 3. Таблица - заполняет середину
        this.Controls.Add(bottomPanel);      // 4. Панель результатов - над статусбаром
        this.Controls.Add(statusStrip);      // 5. Статус бар - самый низ

        // Убеждаемся, что порядок соблюден
        menuStrip.BringToFront();
        toolStrip.BringToFront();
        _dataGridView.BringToFront();
        bottomPanel.BringToFront();
        statusStrip.SendToBack();

        // Обновляем статус
        _dataGridView.RowsAdded += _dataGridView_RowsAdded;
        _dataGridView.CellValueChanged += _dataGridView_CellValueChanged;
        _dataGridView.DataError += _dataGridView_DataError;
        _dataGridView.CellValidating += _dataGridView_CellValidating;

        // Принудительно обновляем макет
        this.PerformLayout();
    }

    private void _dataGridView_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        // if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
        // !_dataGridView.Rows[e.RowIndex].IsNewRow)
        // {
        //     string columnName = _dataGridView.Columns[e.ColumnIndex].Name;
        //     string value = e.FormattedValue?.ToString()?.Trim();

        //     if (string.IsNullOrEmpty(value))
        //     {
        //         _dataGridView.Rows[e.RowIndex].ErrorText =
        //             "Значение не может быть пустым";
        //         e.Cancel = true;
        //         return;
        //     }

        //     if (!double.TryParse(value, out double result))
        //     {
        //         _dataGridView.Rows[e.RowIndex].ErrorText =
        //             $"В колонке '{columnName}' должно быть число";
        //         e.Cancel = true;
        //     }
        //     else
        //     {
        //         _dataGridView.Rows[e.RowIndex].ErrorText = "";
        //     }
        // }
    }

    private void _dataGridView_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        MessageBox.Show(
        $"Ошибка в ячейке: {e.Exception.Message}\n" +
        $"Пожалуйста, введите корректное значение.",
        "Ошибка ввода данных",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    );
    }

    // Метод для проверки всех данных перед расчетом
    private bool ValidateAllData()
    {
        bool isValid = true;

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            if (row.IsNewRow) continue;

            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    cell.ErrorText = "Обязательное поле";
                    isValid = false;
                }
                else if (!double.TryParse(cell.Value.ToString(), out _))
                {
                    cell.ErrorText = "Должно быть число";
                    isValid = false;
                }
            }
        }

        if (!isValid)
        {
            MessageBox.Show("Таблица содержит ошибки. Исправьте их перед расчетом.",
                "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        return isValid;
    }

    private void ChartButton_Click(object? sender, EventArgs e)
    {
        if (countArgs == 1 && _functionModel != null && xData != null && yData != null)
        {
            ChartsGenerator chartsGenerator = new ChartsGenerator(_functionModel.Function);
            chartsGenerator.PlotAndSave(xData, yData, _functionModel.CoeffFunction);
        }
        else
        {
            MessageBox.Show($"Количество аргументов : {countArgs}!\nГрафик рисуется только для двумерных моделей!",
            "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void CalculateButton_Click(object? sender, EventArgs e)
    {
        //проверки

        OptimalFunctionFinder finder = new OptimalFunctionFinder(_appSettings.GetFunctionComponents());
        _statisticModels = new StatisticModels(finder.FindOptimalFunction(xData, yData, _appSettings.Tolerance));

        LoadResultLabel();

        chartButton.Enabled = true;
    }

    private void LoadResultLabel()
    {
        if (_statisticModels != null)
        {
            if (_appSettings.BestModel)
            {
                _functionModel = _statisticModels.GetMaxDetermModel();
            }
            else if (_appSettings.MostCompactThreshold)
            {
                _functionModel = _statisticModels.GetMostCompactEquation(_appSettings.Threshold);
            }

            functionValueLabel.Text = _functionModel.ToString();
            determValueLabel.Text = _functionModel.DeterminationCoeff.ToString();
            iterationsValueLabel.Text = _functionModel.IterConvergence.ToString();
            componentsValueLabel.Text = _functionModel.CountComponents.ToString();

        }
    }

    private void ShowSettingsForm()
    {
        var newSettings = _appSettings;

        using (var settingsForm = new SettingsForm(newSettings))
        {
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _appSettings = newSettings;
            }
        }
    }

    private void ApplySettings()
    {

    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void ShowArgsCountDialog()
    {
        using (var dialog = new Form
        {
            Text = "Количество аргументов",
            Size = new Size(300, 200),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        })
        {
            var label = new Label
            {
                Text = "Введите количество аргументов (X):",
                Location = new Point(20, 20),
                Size = new Size(250, 30)
            };

            var numericUpDown = new NumericUpDown
            {
                Location = new Point(20, 60),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 20,
                Value = 3
            };

            var okButton = new Button
            {
                Text = "OK",
                Location = new Point(50, 110),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 110),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            dialog.Controls.AddRange(new Control[] { label, numericUpDown, okButton, cancelButton });
            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //SetupColumns((int)numericUpDown.Value);
                //AddInitialRow();
                countArgs = (int)numericUpDown.Value;
                InitEmptyData();
            }
        }
    }

    private void LoadFromFile()
    {
        using (var openFileDialog = new OpenFileDialog())
        {
            // Настройки диалога
            openFileDialog.Title = "Выберите файл Excel";
            openFileDialog.Filter = "Excel файлы (*.xlsx; *.xls)|*.xlsx;*.xls|Все файлы (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false; // Только один файл

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _excelFilePath = openFileDialog.FileName;

                LoadDataInTable();

            }
        }
    }

    private void LoadDataInTable()
    {
        if (string.IsNullOrEmpty(_excelFilePath) || !File.Exists(_excelFilePath))
        {
            MessageBox.Show("Сначала выберите файл Excel", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            ExcelReader reader = new ExcelReader(_excelFilePath);
            (xData, yData) = reader.ExcelFileRead();
            countArgs = xData[0].Count;
            if (xData != null && yData != null)
            {
                //UpdateDataInTable();
                UpdateData();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveToFile()
    {
        using (var saveFileDialog = new SaveFileDialog())
        {
            // Настройки диалога
            saveFileDialog.Title = "Сохранение данных в формате Excel";
            saveFileDialog.Filter = "Excel файлы (*.xlsx; *.xls)|*.xlsx;*.xls|Все файлы (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            // Начальная директория
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelPrinter printer = new ExcelPrinter(saveFileDialog.FileName);
                printer.SaveExcelFile(xData, yData);
            }
        }
    }

    private void ClearData()
    {
        if (_dataGridView.ColumnCount > 0)
        {
            var result = MessageBox.Show("Очистить все данные?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ShowArgsCountDialog();
            }
        }
        else
        {
            ShowArgsCountDialog();
        }

    }

    private void InitEmptyData()
    {
        if (xData != null && yData != null)
        {
            xData.Clear();
            yData.Clear();
        }
        else
        {
            xData = new List<List<double>>();
            yData = new List<double>();
        }

        for (int i = 0; i < 5; i++)
        {
            AddEmptyRow();

        }
    }

    private void AddEmptyRow()
    {
        List<double> zeroArgsData = new List<double>();

        for (int i = 0; i < countArgs; i++)
        {
            zeroArgsData.Add(0.0d);
        }

        xData.Add(zeroArgsData);
        yData.Add(0.0d);

        UpdateData();
    }
    private void _dataGridView_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        Console.WriteLine($"col : {e.ColumnIndex} - row : {e.RowIndex}");
        if (e.ColumnIndex > -1 && e.RowIndex >= -1)
        {
            if (xData != null && yData != null)
            {
                if (e.ColumnIndex == countArgs)
                    yData[e.RowIndex] = Convert.ToDouble(_dataGridView[e.ColumnIndex, e.RowIndex].Value);
                else
                    xData[e.RowIndex][e.ColumnIndex] = Convert.ToDouble(_dataGridView[e.ColumnIndex, e.RowIndex].Value);

                UpdateData();
            }
        }
    }

    private void _dataGridView_RowsAdded(object? sender, DataGridViewRowsAddedEventArgs e)
    {
        for (int i = 0; i < e.RowCount; i++)
        {
            int rowIndex = e.RowIndex + i;
            Console.WriteLine($"Добавлена строка {rowIndex}");

            // Автоматическая нумерация
            _dataGridView.Rows[rowIndex].HeaderCell.Value = (rowIndex + 1).ToString();
        }
    }

    private void DeleteSelectedRows()
    {
        if (_dataGridView.SelectedRows.Count > 0)
        {
            List<int> selectedIndices = _dataGridView.SelectedRows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .Select(row => row.Index)
                .ToList();

            foreach (int index in selectedIndices)
            {
                xData.RemoveAt(index);
                yData.RemoveAt(index);
            }

            UpdateData();
        }
        else
        {
            MessageBox.Show("Выберите строки для удаления!", "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void CopySelectedCells()
    {
        if (_dataGridView.GetCellCount(DataGridViewElementStates.Selected) > 0)
        {
            Clipboard.SetDataObject(_dataGridView.GetClipboardContent());
        }
    }

    private void PasteToCells()
    {
        throw new NotImplementedException();
    }

    private void ClearSelectedCells()
    {
        foreach (DataGridViewCell cell in _dataGridView.SelectedCells)
        {
            cell.Value = 0.0d;
        }
    }

    private void ShowStatistics()
    {
        throw new NotImplementedException();
    }

    private DataTable CreateDataTable(List<List<double>> xData, List<double> yData)
    {
        DataTable table = new DataTable();

        for (int i = 0; i < countArgs; i++)
            table.Columns.Add($"x{i + 1}", typeof(double));
        table.Columns.Add("f(x)", typeof(double));

        for (int i = 0; i < xData.Count; i++)
        {
            DataRow row = table.NewRow();
            for (int j = 0; j < xData[i].Count; j++)
                row[j] = xData[i][j];
            row[xData[0].Count] = yData[i];
            table.Rows.Add(row);
        }

        return table;
    }

    public void UpdateData()
    {
        _dataGridView.DataSource = null;
        _dataGridView.Columns.Clear();
        _dataGridView.Rows.Clear();

        DataTable newTable = CreateDataTable(xData, yData);

        _dataGridView.DataSource = newTable;

        ConsolePrinter.PrintInputData(xData, yData);
    }

    public void AddRow(List<double> xValues, double yValue)
    {
        DataTable table = _dataGridView.DataSource as DataTable;
        if (table != null)
        {
            DataRow row = table.NewRow();
            for (int i = 0; i < xValues.Count; i++)
                row[i] = xValues[i];
            row[xValues.Count] = yValue;
            table.Rows.Add(row);
        }
    }
}
