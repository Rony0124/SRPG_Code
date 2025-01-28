using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TSoft.Data.Skill;
using TSoft.InGame.CardSystem;
using UnityEngine;

namespace TSoft.InGame.Player
{
    public partial class PlayerController
    {
        public Action<CardPattern> onPatternSelected;
        
        [Serializable]
        public class CardPattern
        {
            public CardPatternType PatternType;
            public float Modifier;
            public SkillSO skill;
        }
        
        //test 용으로 inspector에서 편집 가능하도록 설정
        [Header("Card Pattern")]
        [SerializeField][TableList]
        private List<CardPattern> defaultCardPatterns;
            
        private List<CardPattern> cardPatterns;

        private CardPattern currentPattern;

        public CardPattern CurrentPattern
        {
            get => currentPattern;
            set
            {
                if (value is not null)
                {
                    onPatternSelected?.Invoke(value);
                }
                
                currentPattern = value;
            }
        }
        
        private bool[] grade;

        public Dictionary<CardPatternType, ParticleSystem> particleDictionary;

        private void InitPattern()
        {
            if (particleDictionary is {Count: > 0})
            {
                foreach (var ps in particleDictionary.Values)
                {
                    Destroy(ps.gameObject);
                }
            }
            
            cardPatterns = new();
            particleDictionary = new();
            currentPattern = new();
            
            foreach (var defaultCardPattern in defaultCardPatterns)
            {
                var particleObj = Instantiate(defaultCardPattern.skill.skillParticleObj, transform);
                var particle = particleObj.GetComponent<ParticleSystem>();
                
                particleDictionary.Add(defaultCardPattern.PatternType, particle);
                cardPatterns.Add(defaultCardPattern);
            }
        }
        
        private void CheckCardPatternOnHand()
        {
            if (currentPokerCardSelected == null || currentPokerCardSelected.Count == 0)
                return;

            grade = new bool[10];
            grade[(int)CardPatternType.None] = true;
            grade[(int)CardPatternType.HighCard] = true;
            
            int[] suits = new int[5]; 
            int[] numbers = new int[15];
            
            for(int i=0; i< currentPokerCardSelected.Count; i++) {
                PokerCard card = currentPokerCardSelected[i];
                // 카드 숫자에 따른 count
                numbers[card.cardData.number]++;
                // Ace의 경우 
                if(card.cardData.number == 1) numbers[14]++;
                // 카드 모양에 따른 count
                switch (card.cardData.type) {
                    case CardType.Spade:
                        suits[1]++;
                        break;
                    case CardType.Diamond:
                        suits[2]++;
                        break;
                    case CardType.Club:
                        suits[3]++;
                        break;
                    case CardType.Heart:
                        suits[4]++;
                        break;
                }
            }
            
            bool isStraight = false;
            // Ace를 1, 14로 처리해서 연속된 구간 확인
            for(int i=1; i<=10; i++){
                int temp = 0;
                // 5개의 연속 구간 확인
                for(int j=i; j<i+5; j++){
                    if(numbers[j] != 1) 
                        break;
                    
                    temp++;
                }
                
                if(temp == 5) 
                    isStraight = true;
            }
            
            if(isStraight) 
                grade[5] = true;
            
            bool isFlush = false;
            for(int i=1; i<5; i++) {
                if(suits[i] == 5) 
                    isFlush = true;
            }
            
            if(isFlush) 
                grade[6] = true;
            
            
            // [9] Straight Flush (같은 무늬의 연속된 숫자 5개가 존재)
            if(isFlush && isStraight) 
                grade[9] = true;
            
            int pairCnt = 0;
            int tripleCnt = 0;
            // Ace를 1로 단일 숫자로 처리해서 확인 (14 의미 제외)
            for(int i=1; i<14; i++) {
                // [7] 4 Card (네 개의 같은 숫자가 존재)
                if(numbers[i] == 4) 
                    grade[8] = true;
 
                // 같은 숫자가 2개
                if(numbers[i] == 2) 
                    pairCnt++;
                // 같은 숫자가 3개
                else if(numbers[i] == 3)
                    tripleCnt++;
            }
            
            // [1] 1 Pair (같은 숫자가 한 쌍 존재)
            if(pairCnt == 1) 
                grade[2] = true;
            // [2] 2 Pair (각기 같은 숫자가 두 쌍 존재)
            if(pairCnt == 2) 
                grade[3] = true;
            // [3] Triple (세 개의 같은 숫자가 존재)
            if(tripleCnt == 1) 
                grade[4] = true;
            // [6] Full House (Triple과 Pair가 함께 존재)
            if(tripleCnt == 1 && pairCnt == 1)
                grade[7] = true;
            
            for (int i = 9; i >= 0; i--)
            {
                if (i == 0)
                {
                    break;
                }
                
                if (grade[i])
                {
                    // 현재 패턴 설정
                    CurrentPattern = cardPatterns.Find(pattern => pattern.PatternType == (CardPatternType)i);
                    break;
                }
            }
        }
    }
}
