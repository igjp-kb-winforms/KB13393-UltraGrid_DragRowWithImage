using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UltraGrid_DragRowWithImage
{
    public partial class Form1 : Form
    {
        DragDropIndicatorManager ind;

        public Form1()
        {
            InitializeComponent();

            ultraGrid1.DataSource = getTable();

            ultraGrid1.DisplayLayout.Override.SelectTypeRow = SelectType.Extended;
            ultraGrid1.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
            ultraGrid1.AllowDrop = true;
        }

        private DataTable getTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("col1", typeof(Int32));
            table.Columns.Add("col2", typeof(string));
            table.Columns.Add("col3", typeof(string));
            table.Columns.Add("col4", typeof(string));
            table.Columns.Add("col5", typeof(string));

            for (int i = 0; i < 100; i++)
                table.Rows.Add(new object[] { i, "test" + i.ToString(), "test", "test", "test" });

            return table;
        }

        private void ultraGrid1_SelectionDrag(object sender, CancelEventArgs e)
        {
            //DragDropIndicatorManagerを作成する
            ind = new DragDropIndicatorManager(Orientation.Horizontal, null);
            List<UIElement> elements = new List<UIElement>();
            foreach (UltraGridRow row in ultraGrid1.Selected.Rows)
            {
                elements.Add(row.GetUIElement());
            }
            ind.InitializeDragIndicator(elements.ToArray());
            ind.DragIndicatorOffset = new Point(-50, 0);
            ind.ShowDragIndicator(System.Windows.Forms.Control.MousePosition);

            //UltraGridのドラッグを開始する
            ultraGrid1.DoDragDrop(ultraGrid1.Selected.Rows, DragDropEffects.Move);
        }

        private void ultraGrid1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            UltraGrid grid = sender as UltraGrid;
            Point pointInGridCoords = grid.PointToClient(new Point(e.X, e.Y));
            // スクロール位置の調整
            if (pointInGridCoords.Y < 20)
                grid.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
            else if (pointInGridCoords.Y > grid.Height - 20)
                grid.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);

            if (ind != null)
            {
                //マウスに合わせてDragDropIndicatorManagerを移動させる
                ind.ShowDragIndicator(System.Windows.Forms.Control.MousePosition);
            }
        }

        private void ultraGrid1_DragDrop(object sender, DragEventArgs e)
        {
            int dropIndex;
            UltraGrid grid = sender as UltraGrid;

            // ドロップされた位置を取得する
            UIElement uieOver = grid.DisplayLayout.UIElement.ElementFromPoint(grid.PointToClient(new Point(e.X, e.Y)));

            // ドロップ位置の行を取得する
            UltraGridRow ugrOver = uieOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;
            if (ugrOver != null)
            {
                dropIndex = ugrOver.Index;

                // 選択行を取得する
                SelectedRowsCollection SelRows = (SelectedRowsCollection)e.Data.GetData(typeof(SelectedRowsCollection));
                foreach (UltraGridRow aRow in SelRows)
                {
                    //選択された行をドロップ領域に移動します
                    ultraGrid1.Rows.Move(aRow, dropIndex);
                }
            }
            //DragDropIndicatorManagerを非表示とする
            ind.HideDragIndicator();

        }

        private void ultraGrid1_DragLeave(object sender, EventArgs e)
        {
            if (ind != null)
            {
                ind.HideDragIndicator();
            }
        }
    }
}
