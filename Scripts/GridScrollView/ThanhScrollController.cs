using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using System;

namespace ThanhScrollController.Grid
{
    public class ThanhScrollController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private LevelData _levelData;
        public EnhancedScroller scroller;
        public EnhancedScrollerCellView cellViewPrefab;
        private float size = 650f;
        private int numberOfCell;
        private SmallList<ThanhData> _data;
        private int numberOfCellsPerRow = 2;
        
        void Start()
        {
            numberOfCell = _levelData.levels.Count;

            scroller.Delegate = this;

            LoadData();
        }

        private void LoadData()
        {
            // set up some simple data
            _data = new SmallList<ThanhData>();
            for (var i = 0; i < numberOfCell; i ++)
            {
                _data.Add(_levelData.levels[i]);
            }

            // tell the scroller to reload now that we have the data
            scroller.ReloadData();
        }

        #region EnhancedScroller Handlers
            public int GetNumberOfCells(EnhancedScroller scroller)
            {
                return Mathf.CeilToInt((float)_data.Count / (float)numberOfCellsPerRow);
            }
            public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
            {
                return size;
            }
            public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
            {
                ThanhCellView cellView = scroller.GetCellView(cellViewPrefab) as ThanhCellView;

                cellView.name = "Cell " + (dataIndex * numberOfCellsPerRow).ToString() + " to " + ((dataIndex * numberOfCellsPerRow) + numberOfCellsPerRow - 1).ToString();

                cellView.SetData(ref _data, dataIndex * numberOfCellsPerRow);

                return cellView;
            }
        #endregion
    }
}
