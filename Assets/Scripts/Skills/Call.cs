using System.Collections;
using UnityEngine;

namespace Skills
{
    public class Call : Base
    {
        [SerializeField] private GameObject indicatorPrefab;
        private GameObject _indicator;
        private readonly Vector3 _indicatorOffset = new Vector3(0, 1.9f, 0);
        public float BuffPercent { get; private set; }

        protected override void LoadResources()
        {
            base.LoadResources();
            var res = resource as Resources.Buff;
            if (res != null)
                BuffPercent = res.buffPercent;
            
            _indicator = Instantiate(indicatorPrefab);
            _indicator.SetActive(false);
        }
      
        protected override void ApplySkill(Vector3 pos)
        {
            ApplySkillToPlanet(pos, CallSupply);
        }

        protected override void CancelSkill()
        {
            IsOnCooldown = false;
            UnblockButton();
        }
        
        private void CallSupply()
        {
            _indicator.SetActive(true);
            _indicator.transform.position = SelectedPlanet.transform.position + _indicatorOffset;

            var launchPos= FindSpawnPoint(SelectedPlanet);
            var destPos = CalculateDestPos(launchPos, SelectedPlanet);
            var unit = ObjectPool.GetObject(SelectedPlanet.Type, 
                launchPos, 
                Quaternion.LookRotation(destPos-launchPos))
                .GetComponent<Units.Base>();////////////////////////////////////////////////////////////////////////////
            SelectedPlanet.AdjustUnit(unit);
            unit.GoTo(SelectedPlanet, destPos);

            StartCoroutine(HideIndicator());

        }

        private IEnumerator HideIndicator()
        {
            yield return new WaitForSeconds(1.5f);
            _indicator.SetActive(false);
        }
        
        private Vector3 CalculateDestPos(in Vector3 launchPos, Planets.Base destinationPlanet)
        {
            var destPos = destinationPlanet.transform.position;
            var offset = (destPos - launchPos).normalized;
            return destPos - offset * destinationPlanet.OrbitRadius;
        }
        
        //calculate a min way on SCREEN (NOT WORLD) coordinates for supply
        private Vector3 FindSpawnPoint(Planets.Base destination)
        {
            var destPosWorld = destination.transform.position;
            var destPosScreen = MainCamera.WorldToScreenPoint(destPosWorld);
            var destX = destPosScreen.x;
            var destY = destPosScreen.y;
            var destZ = destPosScreen.z;

            //possible points for spawn on screen coordinates
            Vector3[] possiblePoints =
            {
                new Vector3(0,destY,destZ),                                  //from left side
                new Vector3(destX,Screen.height,Control.SkillController.MaxDepth), //from top side
                new Vector3(Screen.width,destY,destZ),                       //from right side
                new Vector3(destX,0,Control.SkillController.MinDepth)              //from bottom side
            };

            var minWayIndex = FindMinWay(in possiblePoints,in destPosScreen);
            
            var result=MainCamera.ScreenToWorldPoint(possiblePoints[minWayIndex]);
            result.y = destPosWorld.y;
            return result;
        }

        //find launch point by calculating distance between possible points and destination point
        private static int FindMinWay(in Vector3[] possiblePoints, in Vector3 destinationPos)
        {            
            int minIndex = 0;
            var min = float.MaxValue;
            int index = 0;
            foreach (var point in possiblePoints)
            {
                var distance = Vector2.Distance(point, destinationPos);
                
                if (distance < min)
                {
                    min = distance;
                    minIndex = index;
                }
                index++;
            }

            return minIndex;
        }
        
    }
}