using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace week3
{
    public class LightsPlugin
    {
        // Mock data for the lights
        private readonly List<LightModel> lights = new()
   {
      new LightModel { Id = 1, Name = "Table Lamp", IsOn = false, Brightness = 100, Hex = "FF0000" },
      new LightModel { Id = 2, Name = "Porch light", IsOn = false, Brightness = 50, Hex = "00FF00" },
      new LightModel { Id = 3, Name = "Chandelier", IsOn = true, Brightness = 75, Hex = "0000FF" }
   };

        [KernelFunction("get_lights")]
        [Description("Gets a list of lights and their current state")]
        [return: Description("An array of lights")]
        public async Task<List<LightModel>> GetLightsAsync( )
        {
            return lights;
        }

        [KernelFunction("get_state")]
        [Description("Gets the state of a particular light")]
        [return: Description("The state of the light")]
        public async Task<LightModel?> GetStateAsync( [Description("The ID of the light")] int id )
        {
            // Get the state of the light with the specified ID
            return lights.FirstOrDefault(light => light.Id == id);
        }

        [KernelFunction("change_state")]
        [Description("Changes the state of the light")]
        [return: Description("The updated state of the light; will return null if the light does not exist")]
        public async Task<LightModel?> ChangeStateAsync( int id, LightModel LightModel )
        {
            var light = lights.FirstOrDefault(light => light.Id == id);

            if (light == null)
            {
                return null;
            }

            // Update the light with the new state
            light.IsOn = LightModel.IsOn;
            light.Brightness = LightModel.Brightness;
            light.Hex = LightModel.Hex;

            return light;
        }
    }
}
