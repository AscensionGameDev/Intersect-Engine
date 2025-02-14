using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio;

public partial class MonoSoundInstance(MonoSoundSource source)
    : MonoAudioInstance<MonoSoundSource, SoundEffectInstance>(source, source.Effect?.CreateInstance());