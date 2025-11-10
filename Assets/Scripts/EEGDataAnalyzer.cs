using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using brainflow;
using brainflow.math;
using System;
using Unity.VisualScripting;
using System.Runtime.Serialization;
public class EEGDataAnalyzer : MonoBehaviour
{
    private BoardShim board_shim = null;
    private int sampling_rate = 0;
    private int[] eeg_channels = null;
    private MLModel mindful_model = null;

    private double sumOfMindfulness = 0.0;
    private double nMindfulness = 0.0;

    // Expose BrainFlow's BoardIds in the inspector but we'll limit the selectable options
    [Tooltip("Select which BrainFlow board to use (Synthetic or NeuroPawn Knight).")]
    public BoardIds board_id = BoardIds.SYNTHETIC_BOARD;

    [Inspectable]
    public string serial_port = "COMx"; // set the serial port of your NeuroPawn Knight Board here

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();

            // use the selected BrainFlow board id (only two options are presented in the custom inspector)
            int board_id_int = (int)board_id;
            if (board_id == BoardIds.NEUROPAWN_KNIGHT_BOARD)
            {
                input_params.serial_port = serial_port; // only needed for serial boards like NeuroPawn
            }

            board_shim = new BoardShim(board_id_int, input_params);

            board_shim.prepare_session();
            board_shim.start_stream(450000);
            sampling_rate = BoardShim.get_sampling_rate(board_id_int);
            eeg_channels = BoardShim.get_eeg_channels(board_id_int);
            Debug.Log("Brainflow streaming was started");

            // Create ML model to predict Mindfulness 
            BrainFlowModelParams mindfulness_params = new BrainFlowModelParams(
                (int)BrainFlowMetrics.MINDFULNESS, (int)BrainFlowClassifiers.DEFAULT_CLASSIFIER);

            mindful_model = new MLModel(mindfulness_params);
            
            mindful_model.prepare();
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
        
        // Get the average band powers
        Tuple<double[], double[]> avg_bands = DataFilter.get_avg_band_powers(data, eeg_channels, sampling_rate, true);
        double[] feature_vector = avg_bands.Item1;  // the actual average bands without std dev

        double mindfulness = mindful_model.predict(feature_vector)[0];
        sumOfMindfulness += mindfulness;
        ++nMindfulness;
    }

    // you need to call release_session and ensure that all resources correctly released
    private void OnDestroy()
    {
        if (board_shim != null)
        {
            try
            {
                mindful_model.release();

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

    // Get the average mindfulness score in percentage, measured from the time
    // this script is first run.
    public int GetMindfulnessScore()
    {
        if (nMindfulness == 0) { return 0; }
        return (int) (sumOfMindfulness / nMindfulness * 100); // [%]
    }
}
