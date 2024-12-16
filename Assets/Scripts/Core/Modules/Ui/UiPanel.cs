using Cysharp.Threading.Tasks;

namespace OneDay.Core.Modules.Ui
{
    public abstract class UiPanel : UiElement
    {
        public virtual UniTask Initialize() => UniTask.CompletedTask;
    }
}