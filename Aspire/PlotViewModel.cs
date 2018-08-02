using OxyPlot;
using System;
using System.Collections.ObjectModel;

namespace Aspire
{
    public class PlotEventArgs : EventArgs
    {
        public enum EventType
        {
            Z_UP,   //Z軸アップ
            Z_DOWN  //Z軸ダウン
        }

        public PlotEventArgs(EventType type)
        {
            OccurredEvent = type;
        }

        public EventType OccurredEvent { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class PlotViewModel
    {
        /// <summary>
        /// イベント
        /// </summary>
        public event EventHandler<PlotEventArgs> ActionOccurred;

        //データ格納配列
        public ObservableCollection<DataPoint> XValues { get; private set; }
//        public ObservableCollection<DataPoint> YValues { get; private set; }
//        public ObservableCollection<DataPoint> ZValues { get; private set; }

        public int MaxCount;            //最大格納数

        private const int THRESHOLD_NOP_COUNT = 10;   //無効区間　10回は無視する
        private const int THRESHOLD_SHUFFLE = 150;    //振った際の判定値

        private int _nopCounter = 0;    //解析無効区間判定カウンター
        private Object _lockObj = new Object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlotViewModel()
        {
            XValues = new ObservableCollection<DataPoint>();
//            YValues = new ObservableCollection<DataPoint>();
//            ZValues = new ObservableCollection<DataPoint>();

//            // Initialize charts with dummy data
//            for (int i = 0; i < 100; i++)
//            {
//                XValues.Add(new DataPoint(i, i));
////              YValues.Add(new DataPoint(i, i*i));
////              ZValues.Add(new DataPoint(i, 100*Math.Sin(i*(Math.PI/180))));
//            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void AddData(double x, double y, double z)
        {
            lock (_lockObj)
            {
                XValues.Add(new DataPoint(XValues.Count, x));
//                YValues.Add(new DataPoint(YValues.Count, y));
//                ZValues.Add(new DataPoint(ZValues.Count, z));

                if (MaxCount == XValues.Count)
                {
                    XValues.RemoveAt(0);
                    reNumbering(XValues);

//                  YValues.RemoveAt(0);
//                  reNumbering(YValues);

//                  ZValues.RemoveAt(0);
//                  reNumbering(ZValues);
                }
#if false
                ParseData();
#endif
            }
        }
#if false
        /// <summary>
        ///
        /// </summary>
        private void ParseData()
        {
            if (_nopCounter > 0)
            {
                _nopCounter--;
            }

            //解析 | Analysis
            if ((ZValues.Count > 3) && (_nopCounter == 0))
            {
                double diff = 0;
                diff = ZValues[ZValues.Count - 1].Y - ZValues[ZValues.Count - 2].Y;

                if ((ZValues[ZValues.Count - 1].Y > ZValues[ZValues.Count - 2].Y) && (Math.Abs(diff) > THRESHOLD_SHUFFLE))
                {
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new PlotEventArgs(PlotEventArgs.EventType.Z_UP));
                }
                else if ((ZValues[ZValues.Count - 1].Y < ZValues[ZValues.Count - 2].Y) && (Math.Abs(diff) > THRESHOLD_SHUFFLE))
                {
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new PlotEventArgs(PlotEventArgs.EventType.Z_DOWN));
                }

                /*
                if (ZValues[ZValues.Count - 1].Y - ZValues[ZValues.Count - 2].Y > THRESHOLD_SHUFFLE)
                {
                    //Z軸アップイベント発生させる
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new ActionOccurredEventArgs(ActionOccurredEventArgs.EventType.Z_UP));
                }
                else if (ZValues[ZValues.Count - 1].Y - ZValues[ZValues.Count - 2].Y < -THRESHOLD_SHUFFLE)
                {
                    //Z軸ダウンイベント発生させる
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new ActionOccurredEventArgs(ActionOccurredEventArgs.EventType.Z_DOWN));
                }
                */
            }
        }
#endif
        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        private void reNumbering(ObservableCollection<DataPoint> values)
        {
            int index = 0;
            for (int cnt = 0; cnt < values.Count; cnt++)
            {
                values[cnt] = new DataPoint(index, values[cnt].Y);
                index++;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void ClearAll()
        {
            XValues.Clear();
//            YValues.Clear();
//            ZValues.Clear();
        }
    }
}
