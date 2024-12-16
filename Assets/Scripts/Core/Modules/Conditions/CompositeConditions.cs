using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace OneDay.Core.Modules.Conditions
{
    public abstract class CompositeCondition: ICondition
    {
        protected List<Func<UniTask<bool>>> Conditions { get; }

        protected CompositeCondition(params ICondition[] conditions) =>
            Conditions = conditions.Select(x => (Func<UniTask<bool>>)x.Evaluate).ToList();

        protected CompositeCondition(params Func<UniTask<bool>>[] conditions) => 
            Conditions = conditions.ToList();

        protected CompositeCondition(params Func<bool>[] conditions) =>
            Conditions = conditions.Select(condition => (Func<UniTask<bool>>)(() => UniTask.FromResult(condition()))).ToList();

        public abstract UniTask<bool> Evaluate();
    }
    
    public class CompositeAndCondition : CompositeCondition
    {
        public CompositeAndCondition(params ICondition[] conditions) : base(conditions){}
        public CompositeAndCondition(params Func<UniTask<bool>>[] conditions) : base(conditions){}
        public CompositeAndCondition(params Func<bool>[] conditions) : base(conditions){}
        public override async UniTask<bool> Evaluate()
        {
            foreach (var condition in Conditions)
                if (!await condition())
                    return false;
            return true;
        }
    }
    
    public class CompositeOrCondition : CompositeCondition
    {
        public CompositeOrCondition(params ICondition[] conditions) : base(conditions){}
        public CompositeOrCondition(params Func<UniTask<bool>>[] conditions) : base(conditions){}
        public CompositeOrCondition(params Func<bool>[] conditions) : base(conditions){}
        public override async UniTask<bool> Evaluate()
        {
            foreach (var condition in Conditions)
                if (await condition())
                    return true;
            return false;
        }
    }
}