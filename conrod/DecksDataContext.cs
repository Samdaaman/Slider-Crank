using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace conrod
{
    public class DecksDataContext
    {
        public CommandStack CurrentCommandStack;
        public CrankService CurrentCrankService;
        public MixerService CurrentMixerService;
        public DecksDataContext(CommandStack commandStack, CrankService crankService, MixerService mixerService)
        {
            CurrentCommandStack = commandStack;
            CurrentCrankService = crankService;
            CurrentMixerService = mixerService;
        }
    }
}
