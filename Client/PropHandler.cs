using CitizenFX.Core;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace gflp10_propplacer
{
    internal class PropHandler : BaseScript
    {

        public static async Task<int> CreateProp(string propName)
        {
            int prop = GetHashKey(propName);

            Vector3 coords = GetEntityCoords(PlayerPedId(), true);

            int obj = CreateObject(prop, coords.X, coords.Y, coords.Z - 1, true, true, true);

            await MoveProp(obj);

            return obj;
        }

        public static async Task MoveProp(int obj)
        {

            FreezeEntityPosition(obj, true);
            
            float speed = 1f;
            
            do
            {
                DisableControls(true);

                // Change Speed with h and q
                if (IsDisabledControlPressed(0, 44) && speed < 3.0f)
                    speed += 0.01f;
                if (IsDisabledControlPressed(0, 46) && speed > 0.0f)
                    speed -= 0.01f;

                // Change rotation with mouse left and mouse right
                if (IsDisabledControlPressed(0, 24))
                    SetEntityHeading(obj, GetEntityHeading(obj) - speed);

                if (IsDisabledControlPressed(0, 25))
                    SetEntityHeading(obj, GetEntityHeading(obj) + speed);

                // arrow left
                if (IsControlPressed(0, 174))
                {
                    Vector3 offset = GetOffsetFromEntityInWorldCoords(obj, (-0.01f) * speed, 0.0f, 0.0f);
                    SetEntityCoordsNoOffset(obj, offset.X, offset.Y, offset.Z, false, false, false);
                }

                // arrow right
                if (IsControlPressed(0, 175))
                {
                    Vector3 offset = GetOffsetFromEntityInWorldCoords(obj, (0.01f) * speed, 0.0f, 0.0f);
                    SetEntityCoordsNoOffset(obj, offset.X, offset.Y, offset.Z, false, false, false);
                }
                // arrow up
                if (IsDisabledControlPressed(0, 172))
                {
                    Vector3 offset = GetOffsetFromEntityInWorldCoords(obj, 0.0f, (-0.01f) * speed, 0.0f);
                    SetEntityCoordsNoOffset(obj, offset.X, offset.Y, offset.Z, false, false, false);
                }
                // arrow down
                if (IsDisabledControlPressed(0, 173))
                {
                    Vector3 offset = GetOffsetFromEntityInWorldCoords(obj, 0.0f, (0.01f) * speed, 0.0f);
                    SetEntityCoordsNoOffset(obj, offset.X, offset.Y, offset.Z, false, false, false);
                }

                await Delay(1);
            } while (!IsControlJustReleased(0, 191));

            DisableControls(false);
        }

        private static void DisableControls(bool disable)
        {
            DisableControlAction(0, 24, disable); // Attack
            DisableControlAction(0, 257, disable); // Attack 2
            DisableControlAction(0, 25, disable); // Aim
            DisableControlAction(0, 263, disable); // Melee Attack 1
            DisableControlAction(0, 45, disable); // Reload
            DisableControlAction(0, 264, disable); // Disable melee
            DisableControlAction(0, 257, disable); // Disable melee
            DisableControlAction(0, 140, disable); // Disable melee
            DisableControlAction(0, 141, disable); // Disable melee
            DisableControlAction(0, 142, disable); // Disable melee
            DisableControlAction(0, 143, disable); // Disable melee
            DisableControlAction(0, 14, disable); // Disable weapon wheel
            DisableControlAction(0, 15, disable); // Disable weapon wheel
            DisableControlAction(0, 16, disable); // Disable weapon wheel
            DisableControlAction(0, 17, disable); // Disable weapon wheel
            DisableControlAction(0, 261, disable); // Disable weapon wheel
            DisableControlAction(0, 262, disable); // Disable weapon wheel
            DisableControlAction(0, 44, disable); // Cover
        }
    }
}
