using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using brainflow;
using brainflow.math;
using System;
public class SimpleGetData : MonoBehaviour
{
    private BoardShim board_shim = null;
    private int sampling_rate = 0;
    private int[] eeg_channels = null;
    public string serial_port = "COMx"; // set the serial port of your NeuroPawn Knight Board here

    private Tuple<double, double> alpha_band = new Tuple<double, double>(7.5, 12.5);
    private Tuple<double, double> beta_band = new Tuple<double, double>(13.0, 30.0);

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();

            // int board_id = (int) BoardIds.NEUROPAWN_KNIGHT_BOARD; // By default use NeuroPawn Knight Board
            int board_id = (int) BoardIds.SYNTHETIC_BOARD; // use Synthetic Board for test mode
            
            // input_params.serial_port = serial_port; 
            
            board_shim = new BoardShim(board_id, input_params);

            board_shim.prepare_session();
            board_shim.start_stream(450000);
            sampling_rate = BoardShim.get_sampling_rate(board_id);
            eeg_channels = BoardShim.get_eeg_channels(board_id);
            Debug.Log("Brainflow streaming was started");
        }
        catch (BrainFlowError e)
        {
            Debug.Log(e);
            board_shim = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (board_shim == null)
        {
            return;
        }
        int number_of_data_points = sampling_rate * 4;
        double[,] data = board_shim.get_current_board_data(number_of_data_points);
        // Debug.Log("Num elements: " + data.GetLength(1));

        // Get the average band powers
        Tuple<double[], double[]> avg_bands = DataFilter.get_avg_band_powers(data, eeg_channels, sampling_rate, true);
        double[] feature_vector = avg_bands.Item1;  // the actual average bands without std dev

        // Create two ML models to predict Mindfulness and Restfulness 
        BrainFlowModelParams mindfulness_params = new BrainFlowModelParams(
            (int)BrainFlowMetrics.MINDFULNESS, (int)BrainFlowClassifiers.DEFAULT_CLASSIFIER);
        BrainFlowModelParams restfulness_params = new BrainFlowModelParams(
            (int)BrainFlowMetrics.RESTFULNESS, (int)BrainFlowClassifiers.DEFAULT_CLASSIFIER);

        MLModel mindful_model = new MLModel(mindfulness_params);
        MLModel restful_model = new MLModel(restfulness_params);

        mindful_model.prepare();
        restful_model.prepare();

        // double mindfulness = mindful_model.predict(feature_vector)[0] * 100;
        // double restfulness = restful_model.predict(feature_vector)[0] * 100;
        
        double[] m_arr = mindful_model.predict(feature_vector);
        double[] r_arr = restful_model.predict(feature_vector);

        int[] mi_arr = new int[m_arr.Length];
        int[] ri_arr = new int[r_arr.Length];

        for (int i = 0; i < feature_vector.Length; i++)
        {
            mi_arr[i] = (int)(m_arr[i] * 100);
            ri_arr[i] = (int)(r_arr[i] * 100);
        }

        mindful_model.release();
        restful_model.release();

        // Debug.Log("Mindfulness: " + mindfulness + "\t Restfulness: " + restfulness);
        Debug.Log("Mindfulness array: " + string.Join(", ", mi_arr));
        Debug.Log("Restfulness array: " + string.Join(", ", ri_arr));

    }

    // you need to call release_session and ensure that all resources correctly released
    private void OnDestroy()
    {
        if (board_shim != null)
        {
            try
            {
                board_shim.stop_stream();
                board_shim.release_session();
            }
            catch (BrainFlowError e)
            {
                Debug.Log(e);
            }
            Debug.Log("Brainflow streaming was stopped");
        }
    }
}
