using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorchSharp;

namespace BettaML.Models
{
    public class VisualModel
    {
        public torch.nn.Module Model { get; private set; }

        public VisualModel(string modelPath)
        {
            Model = torch.jit.load(modelPath);
            Model.eval();
        }

        public torch.Tensor Classify(torch.Tensor input)
        {
            return Model.forward(input);
        }
    }
}
