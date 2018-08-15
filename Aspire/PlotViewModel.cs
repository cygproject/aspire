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
        public ObservableCollection<DataPoint> Data { get; private set; }
        public ObservableCollection<DataPoint> Average { get; private set; }
#if NOT_USED
        public ObservableCollection<DataPoint> ZValues { get; private set; }
#endif
        /// <summary>
        /// 最大データ時間枠（秒） | Maximum data time frame (in seconds)
        /// </summary>
        public int TimeFrame;

        /// <summary>
        /// Time interval per data in msec
        /// (50.0, 100.0, 200.0, 500.0 or 1000.0 msec)
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
            Data = new ObservableCollection<DataPoint>();
            Average = new ObservableCollection<DataPoint>();
#if NOT_USED
            ZValues = new ObservableCollection<DataPoint>();
#endif

#if DEBUG
            // Initialize plot with dummy data
            for (int i = 0; i < 100; i++)
            {
                Data.Add(new DataPoint(i, i * Math.Sin(i)));
                Average.Add(new DataPoint(i, i * Math.Cos(i)));
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
                Data.Add(new DataPoint(Interval * Data.Count, yVal));
                Debug.Print("{0},{1}", Data[Data.Count - 1].X, Data[Data.Count - 1].Y);

                for (int cnt = 0; cnt < Data.Count; cnt++)
                {
                    sum += Data[cnt].Y;
                }
                ave = sum / Data.Count;

                Debug.Print("Count:" + Data.Count);

                Average.Add(new DataPoint(Interval * Average.Count, ave));
#if NOT_USED
                ZValues.Add(new DataPoint(ZValues.Count, z));
#endif
                if (TimeFrame <= Interval * Data.Count)
                {
                    Data.RemoveAt(0);
                    
                    UpdateIndex(Data);

                    Average.RemoveAt(0);
                    UpdateIndex(Average);
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
            Data.Clear();
            Average.Clear();
#if NOT_USED
            ZValues.Clear();
#endif
        }
    }
}
