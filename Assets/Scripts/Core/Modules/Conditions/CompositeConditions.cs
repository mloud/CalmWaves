using System;
using Cysharp.Threading.Tasks;

namespace OneDay.Core.Modules.Conditions
{
    public abstract class CompositeCondition: ICondition
    {
        protected Func<UniTask<bool>> ConditionA { get; }
        protected Func<UniTask<bool>> ConditionB { get; }
        
        protected CompositeCondition(ICondition conditionA, ICondition conditionB)
        {
            ConditionA = conditionA.Evaluate;
            ConditionB = conditionB.Evaluate;
        }
        
        protected CompositeCondition(Func<UniTask<bool>> conditionA, Func<UniTask<bool>> conditionB)
        {
            ConditionA = conditionA;
            ConditionB = conditionB;
        }

        public abstract UniTask<bool> Evaluate();
    }
    
    public class CompositeAndCondition : CompositeCondition
    {
        public CompositeAndCondition(ICondition conditionA, ICondition conditionB) : 
            base(conditionA, conditionB) {}
        
        public CompositeAndCondition(Func<UniTask<bool>> conditionA, Func<UniTask<bool>> conditionB) :
            base(conditionA, conditionB) {}
        
        public override async UniTask<bool> Evaluate() => 
            await ConditionA() && await ConditionB();
    }
    
    public class CompositeOrCondition : CompositeCondition
    {
        public CompositeOrCondition(ICondition conditionA, ICondition conditionB) : 
            base(conditionA, conditionB) {}
        
        public CompositeOrCondition(Func<UniTask<bool>> conditionA, Func<UniTask<bool>> conditionB) :
            base(conditionA, conditionB) {}
        public override async UniTask<bool> Evaluate() => 
            await ConditionA() || await ConditionB();
    }
}