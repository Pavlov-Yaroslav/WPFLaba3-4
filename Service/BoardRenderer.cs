using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp1.Service;

namespace WpfApp1.Services
{
    public class BoardRenderer
    {
        public void RenderBoard(WrapPanel panel, Board board)
        {
            panel.Children.Clear();

            int cellSize = CalculateCellSize(board.Size);

            for (int i = 0; i < board.Size; i++)
            {
                var cell = board.Cells[i];
                panel.Children.Add(CreateCellVisual(cell, i, board.Size, cellSize));
            }
        }

        private int CalculateCellSize(int boardSize)
        {
            if (boardSize <= 30) return 60;
            if (boardSize <= 40) return 55;
            if (boardSize <= 50) return 50;
            if (boardSize <= 60) return 45;
            return 40;
        }

        public void UpdatePlayerMarkers(WrapPanel panel, Board board, List<Player> players)
        {
            foreach (var child in panel.Children)
            {
                if (child is Border border && border.Child is Grid grid)
                {
                    for (int i = grid.Children.Count - 1; i >= 0; i--)
                    {
                        if (grid.Children[i] is StackPanel || grid.Children[i] is Ellipse)
                            grid.Children.RemoveAt(i);
                    }
                }
            }

            var playersByPosition = players
                .Where(p => p.Position >= 0 && p.Position < board.Size)
                .GroupBy(p => p.Position)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var position in playersByPosition.Keys)
            {
                if (position < panel.Children.Count)
                {
                    if (panel.Children[position] is Border border && border.Child is Grid grid)
                    {
                        var playersOnCell = playersByPosition[position];
                        grid.Children.Add(CreatePlayersMarker(playersOnCell));
                    }
                }
            }
        }

        private Border CreateCellVisual(ICell cell, int position, int boardSize, int cellSize)
        {
            return new Border
            {
                Width = cellSize,
                Height = cellSize,
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Background = GetCellColor(cell, position, boardSize),
                ToolTip = CreateCellTooltip(cell, position, boardSize),
                Child = CreateCellContent(cell, position, boardSize, cellSize)
            };
        }

        private Grid CreateCellContent(ICell cell, int position, int boardSize, int cellSize)
        {
            var grid = new Grid();

            int fontSize = cellSize > 50 ? 10 : 8;
            int iconSize = cellSize > 50 ? 18 : 14;

            grid.Children.Add(new TextBlock
            {
                Text = (position + 1).ToString(),
                FontSize = fontSize,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(3),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            });

            string icon = GetCellIcon(cell, position, boardSize);
            if (!string.IsNullOrEmpty(icon))
            {
                grid.Children.Add(new TextBlock
                {
                    Text = icon,
                    FontSize = iconSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Black
                });
            }

            return grid;
        }

        private UIElement CreatePlayersMarker(List<Player> players)
        {
            if (players.Count == 1)
            {
                return new Ellipse
                {
                    Width = 15,
                    Height = 15,
                    Fill = BrushHelper.GetBrush(players[0].Color),
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = CreatePlayerTooltip(players)
                };
            }
            else
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = CreatePlayerTooltip(players)
                };

                foreach (var player in players)
                {
                    stackPanel.Children.Add(new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Fill = BrushHelper.GetBrush(player.Color),
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5,
                        Margin = new Thickness(1)
                    });
                }

                return stackPanel;
            }
        }

        private ToolTip CreatePlayerTooltip(List<Player> players)
        {
            string tooltipText;

            if (players.Count == 1)
            {
                tooltipText = $"{players[0].Name}\nПозиция: {players[0].Position + 1}";
            }
            else
            {
                tooltipText = $"Игроки на ячейке {players[0].Position + 1}:\n";
                foreach (var player in players)
                {
                    tooltipText += $"• {player.Name}\n";
                }
                tooltipText = tooltipText.TrimEnd('\n');
            }

            return new ToolTip { Content = tooltipText };
        }

        private Brush GetCellColor(ICell cell, int position, int boardSize)
        {
            if (position == 0) return Brushes.Gold;
            if (position == boardSize - 1) return Brushes.Silver;
            if (cell is ForwardCell) return Brushes.LightGreen;
            if (cell is BackCell) return Brushes.LightCoral;
            if (cell is SkipCell) return Brushes.LightBlue;
            return Brushes.White;
        }

        private string GetCellIcon(ICell cell, int position, int boardSize)
        {
            if (position == 0) return "Start";
            if (position == boardSize - 1) return "Finish";

            // Иконки для специальных ячеек
            if (cell is ForwardCell) return "↑";
            if (cell is BackCell) return "↓";
            if (cell is SkipCell) return "X";

            return "";
        }

        private ToolTip CreateCellTooltip(ICell cell, int position, int boardSize)
        {
            string type = "Простая";
            if (cell is ForwardCell) type = "Вперед +2";
            else if (cell is BackCell) type = "Назад -3";
            else if (cell is SkipCell) type = "Пропуск хода";
            else if (position == 0) type = "СТАРТ";
            else if (position == boardSize - 1) type = "ФИНИШ";

            return new ToolTip { Content = $"Ячейка {position + 1}\nТип: {type}" };
        }
    }
}