using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorchSharp;
using static TorchSharp.torch.nn;

namespace BettaML.Models
{

    public static class ModelFactory
    {
        public static torch.nn.Module CreateModel(string modelType, int numClasses)
        {
            return modelType.ToLower() switch
            {
                "resnet" => torchvision.models.resnet18(pretrained: true).ReplaceFinalLayer(numClasses),
                "alexnet" => torchvision.models.alexnet(pretrained: true).ReplaceFinalLayer(numClasses),
                _ => throw new ArgumentException("Unsupported model type")
            };
        }
    }

    public static class ModelExtensions
    {
        public static Module ReplaceFinalLayer(this Module model, int numClasses)
        {
            if (model.fc != null) model.fc = Linear(model.fc.in_features, numClasses);
            return model;
        }
    }
}
