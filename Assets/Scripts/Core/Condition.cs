using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition 
    {
        [SerializeField] private Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction dis in and)
            {
                if (!dis.Check(evaluators))
                {
                    return false;
                }
            }
            return true;
        }
        [System.Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;
            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate pre in or)
                {
                    if (pre.Check(evaluators))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [System.Serializable]
        public class Predicate
        {
            [SerializeField] private string _predicate;
            [SerializeField] private string[] _parameters;
            [SerializeField] private bool _negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(_predicate, _parameters);
                    if (result == null) continue;
                    if (result == _negate) return false;
                }
                return true;
            }
        }
        
        
    }
}
