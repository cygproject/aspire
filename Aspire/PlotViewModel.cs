using OxyPlot;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        public ObservableCollection<DataPoint> YValues { get; private set; }
#if NOT_USED
        public ObservableCollection<DataPoint> ZValues { get; private set; }
#endif
        public int MaxCount;            //最大格納数

        private readonly Object LockObj = new Object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlotViewModel()
        {
            XValues = new ObservableCollection<DataPoint>();
            YValues = new ObservableCollection<DataPoint>();
#if NOT_USED
            ZValues = new ObservableCollection<DataPoint>();
#endif

#if DEBUG_ONLY
            // Initialize plot with dummy data
            for (int i = 0; i < 100; i++)
            {
                XValues.Add(new DataPoint(i, i));
#if NOT_USED
                YValues.Add(new DataPoint(i, i*i));
                ZValues.Add(new DataPoint(i, 100*Math.Sin(i*(Math.PI/180))));
#endif
            }
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void AddData(double x, double y, double z)
        {
            double ave = 0.0;
            double sum = 0.0;

            lock (LockObj)
            {
                XValues.Add(new DataPoint(XValues.Count, x));

                for (int cnt = 0; cnt < XValues.Count; cnt++)
                {
                    sum += XValues[cnt].Y;
                }
                ave = sum / XValues.Count;

                Debug.Print("Count:" + XValues.Count);

                YValues.Add(new DataPoint(YValues.Count, ave));
#if NOT_USED
                ZValues.Add(new DataPoint(ZValues.Count, z));
#endif
                if (MaxCount == XValues.Count)
                {
                    XValues.RemoveAt(0);
                    UpdateIndex(XValues);

                    YValues.RemoveAt(0);
                    UpdateIndex(YValues);
#if NOT_USED
                    ZValues.RemoveAt(0);
                    UpdateIndex(ZValues);
#endif
                }
            }

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        private void UpdateIndex(ObservableCollection<DataPoint> values)
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
            YValues.Clear();
#if NOT_USED
            ZValues.Clear();
#endif
        }
    }
}
