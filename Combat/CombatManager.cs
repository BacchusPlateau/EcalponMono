using System.Collections.Generic;
using System.Linq;

namespace Ecalpon.Combat
{
    public class CombatManager
    {
        // --- The one truth about what's happening right now ---
        public CombatState CurrentState { get; private set; }

        // --- Everyone who participates in this fight ---
        private List<Combatant> _combatants = new List<Combatant>();

        // --- Whose turn is it ---
        private int _currentCombatantIndex = 0;

        // --- Messages for the feedback panel ---
        private List<string> _displayLog = new List<string>();
        public IReadOnlyList<string> RecentMessages => _displayLog;

        // --- Random number generator ---
        private System.Random _rng = new System.Random();

        // =====================================================
        // INITIALIZATION
        // =====================================================

        public void StartCombat(List<Combatant> playerParty,
                                List<Combatant> enemies)
        {
            _combatants.Clear();
            _combatants.AddRange(playerParty);
            _combatants.AddRange(enemies);

            RollInitiative();
            _currentCombatantIndex = 0;

            TransitionTo(CombatState.Initializing);
            TransitionTo(CombatState.PlayerTurn);
        }

        // =====================================================
        // STATE MACHINE — the heart of everything
        // =====================================================

        public void TransitionTo(CombatState newState)
        {
            CurrentState = newState;

            switch (CurrentState)
            {
                case CombatState.PlayerTurn:
                    PrepareCurrentCombatantTurn();
                    break;

                case CombatState.EnemyTurn:
                    PrepareCurrentCombatantTurn();
                    break;

                case CombatState.Victory:
                    AddMessage("Victory! The enemy has been defeated.");
                    break;

                case CombatState.Defeat:
                    AddMessage("Your party has fallen...");
                    break;
            }
        }

        // =====================================================
        // TURN ORDER
        // =====================================================

        private void RollInitiative()
        {
            foreach (var combatant in _combatants)
                combatant.Initiative = _rng.Next(1, 20) + combatant.Level;

            _combatants = _combatants
                .OrderByDescending(c => c.Initiative)
                .ToList();
        }

        private void PrepareCurrentCombatantTurn()
        {
            Combatant current = CurrentCombatant();
            if (current == null)
                return;

            current.HasActedThisTurn = false;
            current.MovesRemaining = current.MaxMoves;

            if (current.IsPlayerControlled)
                AddMessage(current.Name + "'s turn.");
            else
                AddMessage(current.Name + " acts...");
        }

        public Combatant CurrentCombatant()
        {
            if (_combatants.Count == 0)
                return null;

            return _combatants[_currentCombatantIndex];
        }

        public void EndCurrentTurn()
        {
            if (AllEnemiesDefeated())
            {
                TransitionTo(CombatState.Victory);
                return;
            }

            if (AllPlayerCombatantsDefeated())
            {
                TransitionTo(CombatState.Defeat);
                return;
            }

            AdvanceToNextCombatant();
        }

        public string LastMessage()
        {
            if (_displayLog.Count == 0)
                return "";

            return _displayLog[_displayLog.Count - 1];
        }

        private void AdvanceToNextCombatant()
        {
            int attempts = 0;

            do
            {
                _currentCombatantIndex++;

                if (_currentCombatantIndex >= _combatants.Count)
                    _currentCombatantIndex = 0;

                attempts++;

            } while (!_combatants[_currentCombatantIndex].IsAlive
                     && attempts <= _combatants.Count);

            Combatant next = CurrentCombatant();

            System.Diagnostics.Debug.WriteLine("Next combatant: " + next.Name + " index: " + _currentCombatantIndex);

            if (next.IsPlayerControlled)
                TransitionTo(CombatState.PlayerTurn);
            else
                TransitionTo(CombatState.EnemyTurn);
        }

        // =====================================================
        // COMBAT RESOLUTION — AD&D THAC0
        // =====================================================

        public bool RollToHit(Combatant attacker, Combatant defender)
        {
            int roll = _rng.Next(1, 21);
            int needed = attacker.ThacO - defender.ArmorClass;

            if (roll >= needed)
            {
                AddMessage(attacker.Name + " hits " + defender.Name
                    + "! (rolled " + roll + ", needed " + needed + ")");
                return true;
            }
            else
            {
                AddMessage(attacker.Name + " misses " + defender.Name
                    + ". (rolled " + roll + ", needed " + needed + ")");
                return false;
            }
        }

        public int RollDamage(Combatant attacker)
        {
            int damage = _rng.Next(attacker.DamageMin, attacker.DamageMax + 1)
                         + attacker.DamageBonus;

            return damage;
        }

        public void ApplyDamage(Combatant target, int damage)
        {
            target.HitPoints -= damage;

            if (!target.IsAlive)
            {
                AddMessage(target.Name + " has been slain!");
            }
            else
            {
                AddMessage(target.Name + " takes " + damage + " damage."
                    + " (" + target.HitPoints + "/" + target.MaxHitPoints + " HP)");
            }
        }

        // =====================================================
        // VICTORY / DEFEAT CONDITIONS
        // =====================================================

        private bool AllEnemiesDefeated()
        {
            return _combatants
                .Where(c => c.Type == CombatantType.Enemy)
                .All(c => !c.IsAlive);
        }

        private bool AllPlayerCombatantsDefeated()
        {
            return _combatants
                .Where(c => c.IsPlayerControlled)
                .All(c => !c.IsAlive);
        }

        // =====================================================
        // MESSAGE LOG
        // =====================================================

        private void AddMessage(string message)
        {
            _displayLog.Add(message);

            if (_displayLog.Count > 8)
                _displayLog.RemoveAt(0);
        }

        public IEnumerable<Combatant> GetAliveCombatants()
        {
            return _combatants.Where(c => c.IsAlive);
        }
    }
}