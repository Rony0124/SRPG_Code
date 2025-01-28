using System;
using TSoft.InGame;
using TSoft.UI.Core;

namespace TSoft.UI.Views.InGame
{
    public class FieldInfoView : ViewBase
    {
        public Action<float, float> OnDamaged;
        public Action<MonsterController> OnMonsterSpawn;
        
        private enum FieldInfoText
        {
            MonsterNameTxt,
            FieldHpTxt
        }
        
        private TMPro.TextMeshProUGUI txtName;
        private TMPro.TextMeshProUGUI txtHp;
        
        private void Start()
        {
            Bind<TMPro.TextMeshProUGUI>(typeof(FieldInfoText));
            
            txtName = Get<TMPro.TextMeshProUGUI>((int)FieldInfoText.MonsterNameTxt);
            txtHp = Get<TMPro.TextMeshProUGUI>((int)FieldInfoText.FieldHpTxt);
        }
        
        protected override void OnActivated()
        {
            OnDamaged += UpdateMonsterHp;
            OnMonsterSpawn += UpdateOnMonsterSpawn;
        }

        protected override void OnDeactivated()
        {
            OnDamaged -= UpdateMonsterHp;
            OnMonsterSpawn -= UpdateOnMonsterSpawn;
        }

        private void UpdateOnMonsterSpawn(MonsterController monster)
        {
            txtName.text = monster.Data.name;
            txtHp.text = (int)monster.GamePlay.GetAttr(GameplayAttr.Heart) + " / " + (int)monster.GamePlay.GetAttr(GameplayAttr.MaxHeart);
        }

        private void UpdateMonsterHp(float hp, float maxHp)
        {
            txtHp.text = (int)hp + " / " + (int)maxHp;
        }
    }
}
