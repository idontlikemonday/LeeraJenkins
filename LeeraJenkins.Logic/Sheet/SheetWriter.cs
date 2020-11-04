using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Model;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Sheet
{
    public class SheetWriter : ISheetWriter
    {
        private ISheetService _sheetService;

        private string _spreadsheetId = Settings.SpreadsheetId;
        private string _sheetName = Settings.SheetName;
        private int _sheetId = Settings.SheetId;

        private int _maxColumnNum = 26;

        public SheetWriter(ISheetService sheetService)
        {
            _sheetService = sheetService;
        }

        public void WriteValue(string value, string columnName, long sheetRowId)
        {
            var service = _sheetService.GetSheetService();

            var range = $"{_sheetName}!{columnName}{sheetRowId}";
            var valueRange = new ValueRange();

            IList<IList<object>> values = new List<IList<object>>()
            {
                new List<object>() { value }
            };

            valueRange.Values = values;

            var request = service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
            var valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)1;

            request.ValueInputOption = valueInputOption;
            var result = request.Execute();
        }

        public void WriteNewGameValues(List<string> newGameValues, long sheetRowId)
        {
            var service = _sheetService.GetSheetService();

            var range = $"{_sheetName}!A{sheetRowId}";
            var valueRange = new ValueRange();

            IList<IList<object>> values = new List<IList<object>>()
            {
                newGameValues.Select(v => (object)v).ToList()
            };

            valueRange.Values = values;

            var request = service.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
            var valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)1;

            request.ValueInputOption = valueInputOption;
            var result = request.Execute();
        }

        public void DeleteRow(long sheetRowId)
        {
            var service = _sheetService.GetSheetService();

            var deleteRowRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        DeleteDimension = new DeleteDimensionRequest()
                        {
                            Range = new DimensionRange()
                            {
                                 SheetId = _sheetId,
                                 Dimension = "ROWS",
                                 StartIndex = (int)sheetRowId - 1,
                                 EndIndex = (int)sheetRowId
                            }
                        },
                    }
                }
            };

            var request = service.Spreadsheets.BatchUpdate(deleteRowRequest, _spreadsheetId);
            request.Execute();
        }

        public void InsertNewDayRow(long sheetRowId, DateTime day)
        {
            InsertEmptyRow(sheetRowId);
            MergeRow(sheetRowId);

            var dayStr = day.ToTableDayRowFormat();
            WriteNewDayRow(sheetRowId, dayStr);
        }

        public void InsertEmptyRow(long sheetRowId)
        {
            var service = _sheetService.GetSheetService();

            var insertRowRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        InsertDimension = new InsertDimensionRequest()
                        {
                            Range = new DimensionRange()
                            {
                                 SheetId = _sheetId,
                                 Dimension = "ROWS",
                                 StartIndex = (int)sheetRowId - 1,
                                 EndIndex = (int)sheetRowId
                            }
                        },
                    }
                }
            };

            var colorCellsWhiteRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        RepeatCell = new RepeatCellRequest()
                        {
                            Range = new GridRange()
                            {
                                SheetId = _sheetId,
                                StartColumnIndex = 0,
                                EndColumnIndex = _maxColumnNum,
                                StartRowIndex = (int)sheetRowId - 1,
                                EndRowIndex = (int)sheetRowId
                            },
                            Cell = new CellData()
                            {
                                UserEnteredFormat = new CellFormat()
                                {
                                    BackgroundColor = new Color()
                                    {
                                        Red = 1,
                                        Green = 1,
                                        Blue = 1,
                                        Alpha = 0
                                    }
                                }
                            },
                            Fields = "*"
                        }
                    }
                }
            };

            var request = service.Spreadsheets.BatchUpdate(insertRowRequest, _spreadsheetId);
            request.Execute();

            request = service.Spreadsheets.BatchUpdate(colorCellsWhiteRequest, _spreadsheetId);
            request.Execute();
        }

        public void WriteNewDayRow(long sheetRowId, string value)
        {
            var service = _sheetService.GetSheetService();

            var newDayStyleCell = new CellData()
            {
                UserEnteredFormat = new CellFormat()
                {
                    BackgroundColor = new Color()
                    {
                        Red = 0.1484375f,
                        Green = 0.6484375f,
                        Blue = 0.6015625f,
                        Alpha = 0
                    },
                    TextFormat = new TextFormat()
                    {
                        FontSize = 17,
                        Bold = true,
                        ForegroundColor = new Color()
                        {
                            Red = 1,
                            Green = 1,
                            Blue = 1,
                            Alpha = 0
                        }
                    },
                    HorizontalAlignment = "LEFT",
                    VerticalAlignment = "MIDDLE"
                }
            };

            var newDayStyleRowRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        RepeatCell = new RepeatCellRequest()
                        {
                            Range = new GridRange()
                            {
                                SheetId = _sheetId,
                                StartColumnIndex = 0,
                                EndColumnIndex = 19,
                                StartRowIndex = (int)sheetRowId - 1,
                                EndRowIndex = (int)sheetRowId
                            },
                            Cell = newDayStyleCell,
                            Fields = "*"
                        }
                    }
                }
            };

            var cellHeightRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        UpdateDimensionProperties = new UpdateDimensionPropertiesRequest()
                        {
                            Range = new DimensionRange()
                            {
                                 SheetId = _sheetId,
                                 Dimension = "ROWS",
                                 StartIndex = (int)sheetRowId - 1,
                                 EndIndex = (int)sheetRowId
                            },
                            Properties = new DimensionProperties()
                            {
                                PixelSize = 40
                            },
                            Fields = "*"
                        }
                    }
                }
            };

            var request = service.Spreadsheets.BatchUpdate(newDayStyleRowRequest, _spreadsheetId);
            request.Execute();
            request = service.Spreadsheets.BatchUpdate(cellHeightRequest, _spreadsheetId);
            request.Execute();

            WriteValue(value, "A", sheetRowId);
        }

        private void MergeRow(long sheetRowId)
        {
            var service = _sheetService.GetSheetService();

            var mergeRowRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        MergeCells = new MergeCellsRequest()
                        {
                            MergeType = "MERGE_ROWS",
                            Range = new GridRange()
                            {
                                SheetId = _sheetId,
                                StartColumnIndex = 0,
                                EndColumnIndex = _maxColumnNum,
                                StartRowIndex = (int)sheetRowId - 1,
                                EndRowIndex = (int)sheetRowId
                            }
                        }
                    }
                }
            };

            var request = service.Spreadsheets.BatchUpdate(mergeRowRequest, _spreadsheetId);
            request.Execute();
        }

        public void FormatNewGameRow(long sheetRowId)
        {
            var service = _sheetService.GetSheetService();

            var centeredCellValues = new CellData()
            {
                UserEnteredFormat = new CellFormat()
                {
                    HorizontalAlignment = "CENTER",
                    VerticalAlignment = "MIDDLE"
                }
            };

            var centeredCellValuesReuqest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        RepeatCell = new RepeatCellRequest()
                        {
                            Range = new GridRange()
                            {
                                SheetId = _sheetId,
                                StartColumnIndex = (int)RegistrationTableColumnsOrder.Time,
                                EndColumnIndex = (int)RegistrationTableColumnsOrder.MaxPlayers + 1,
                                StartRowIndex = (int)sheetRowId - 1,
                                EndRowIndex = (int)sheetRowId
                            },
                            Cell = centeredCellValues,
                            Fields = "*"
                        }
                    }
                }
            };

            var leftCellValues = new CellData()
            {
                UserEnteredFormat = new CellFormat()
                {
                    HorizontalAlignment = "LEFT",
                    VerticalAlignment = "MIDDLE"
                }
            };

            var leftCellValuesReuqest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        RepeatCell = new RepeatCellRequest()
                        {
                            Range = new GridRange()
                            {
                                SheetId = _sheetId,
                                StartColumnIndex = (int)RegistrationTableColumnsOrder.Name,
                                EndColumnIndex = (int)RegistrationTableColumnsOrder.Host + 1,
                                StartRowIndex = (int)sheetRowId - 1,
                                EndRowIndex = (int)sheetRowId
                            },
                            Cell = leftCellValues,
                            Fields = "*"
                        }
                    }
                }
            };

            var boldCellValues = new CellData()
            {
                UserEnteredFormat = new CellFormat()
                {
                    TextFormat = new TextFormat()
                    {
                        Bold = true
                    }
                }
            };

            var boldCellValuesReuqest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>()
                {
                    new Request()
                    {
                        RepeatCell = new RepeatCellRequest()
                        {
                            Range = new GridRange()
                            {
                                SheetId = _sheetId,
                                StartColumnIndex = (int)RegistrationTableColumnsOrder.Name,
                                EndColumnIndex = (int)RegistrationTableColumnsOrder.Name + 1,
                                StartRowIndex = (int)sheetRowId - 1,
                                EndRowIndex = (int)sheetRowId
                            },
                            Cell = boldCellValues,
                            Fields = "*"
                        }
                    }
                }
            };

            var request = service.Spreadsheets.BatchUpdate(centeredCellValuesReuqest, _spreadsheetId);
            request.Execute();
            request = service.Spreadsheets.BatchUpdate(leftCellValuesReuqest, _spreadsheetId);
            request.Execute();
            request = service.Spreadsheets.BatchUpdate(boldCellValuesReuqest, _spreadsheetId);
            request.Execute();
        }
    }
}
