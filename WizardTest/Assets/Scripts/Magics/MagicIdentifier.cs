using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using UnityEngine;

namespace MachineLearning
{
    public class MagicIdentifier : MonoBehaviour
    {
        public HttpRequest request;
        public string network_id;
        public float tolerance;
        private Network network_data;
        private BasicNetwork network;
        private List<Magic> magics;

        void Start()
        {
            request.Request("Networks/_GetNetwork?id=" + network_id, InitNetwork);
        }

        private IEnumerator InitNetwork(WWW req)
        {
            yield return req;
            network_data = JsonConvert.DeserializeObject<Network>(req.text);
            network = LoadNetwork(network_data);
            request.Request("MagicBuilder/_GetMagicMin?id=" + network_id, InitMagics);
        }

        private BasicNetwork LoadNetwork(Network network_data)
        {
            byte[] byteData = Convert.FromBase64String(network_data.data);
            double[] data = new double[byteData.Length / 8];
            Buffer.BlockCopy(byteData, 0, data, 0, byteData.Length);
            BasicNetwork network = new BasicNetwork();
            network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 30 * 2 * 2));
            for (int i = 0; i < network_data.hidden_count; i++)
            {
                network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, network_data.hidden_length));
            }
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure();
            network.Reset();
            network.DecodeFromArray(data);
            return network;
        }

        public int IdentifyMagic(double[] raw_input)
        {
            List<double> input;
            double max = 0;
            double[] op = new double[4];
            int index = 0;
            for (int i = 0; i < magics.Count; i++)
            {
                input = raw_input.ToList();
                input.AddRange(magics[i].baseline);
                IMLData inputData = new BasicMLData(input.ToArray());
                IMLData output;
                output = network.Compute(inputData);
                op[i] = output[0];
                if (max < output[0])
                {
                    max = output[0];
                    index = i;
                }
            }
            foreach (double o in op)
            {
                Debug.Log(o);
            }
            return (max > tolerance) ? index : -1;
        }

        private IEnumerator InitMagics(WWW req)
        {
            yield return req;
            magics = JsonConvert.DeserializeObject<List<Magic>>(req.text);
        }
    }
}

