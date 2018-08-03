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
#if NOT_USED
        /// <summary>
        /// イベント
        /// </summary>
        public event EventHandler<PlotEventArgs> ActionOccurred;
#endif
        //データ格納配列
        public ObservableCollection<DataPoint> XValues { get; private set; }
        public ObservableCollection<DataPoint> YValues { get; private set; }
#if NOT_USED
        public ObservableCollection<DataPoint> ZValues { get; private set; }
#endif
        /// <summary>
        /// 最大格納数 | Number of data to store (maximum)
        /// </summary>
        public int MaxCount;

        /// <summary>
        /// Approximate time interval per data (0.1 ~ 1.0 second)
        /// </summary>
        public double Interval;

        /// <summary>
        /// 
        /// </summary>
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

#if DEBUG
            // Initialize plot with dummy data
            for (int i = 0; i < 100; i++)
            {
                XValues.Add(new DataPoint(i, i));
                YValues.Add(new DataPoint(i, i*i));
#if NOT_USED
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
        public void AddData(double yVal)
        {
            double ave = 0.0;
            double sum = 0.0;

            lock (LockObj)
            {
                XValues.Add(new DataPoint(Interval * XValues.Count, yVal));

                for (int cnt = 0; cnt < XValues.Count; cnt++)
                {
                    sum += XValues[cnt].Y;
                }
                ave = sum / XValues.Count;

                Debug.Print("Count:" + XValues.Count);

                YValues.Add(new DataPoint(Interval * YValues.Count, ave));
#if NOT_USED
                ZValues.Add(new DataPoint(ZValues.Count, z));
#endif
                if (MaxCount == Interval * XValues.Count)
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
                values[cnt] = new DataPoint(Interval * index, values[cnt].Y);
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
