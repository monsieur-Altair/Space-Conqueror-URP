using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;


namespace AI
{ 
    public class Core : MonoBehaviour
    {
        public static float ScientificCount;
        public Vector3 MainPos { get; private set; }
        public List<List<Planets.Base>> AllPlanets { get; private set; }
        public static int Own, Enemy, Neutral;
        public static Core Instance { get; private set; }

        [SerializeField] private GameObject allActions;

        private Planets.Base _mainPlanet;
        private bool _isActive;
        private IAction _attackByRocket;
        private const float MinDelay = 4.0f;
        private const float MaxDelay = 7.0f;

        
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            // Debug.Log("address "+LocalIPAddress());
            // Debug.Log("addressV4 "+GetIP(ADDRESSFAM.IPv4));
            // Debug.Log("addressV6 "+GetIP(ADDRESSFAM.IPv6));
            // Debug.Log("localV4 "+GetLocalIPv4());
            
            AllPlanets = new List<List<Planets.Base>>();
            

            Own = (int) Planets.Team.Red;
            Enemy = (int) Planets.Team.Blue;
            Neutral = (int) Planets.Team.White;

            Planets.Base.Conquered += AdjustPlanetsList;
        }

        public void Init(List<Planets.Base> planets)
        {
            AllPlanets.Clear();
            
            for (int i = 0; i < 3; i++)
                AllPlanets.Add(new List<Planets.Base>());


            foreach (Planets.Base planet in planets)
            {
                AllPlanets[(int)planet.Team].Add(planet);
            }

            _mainPlanet = AllPlanets[Own][0];
            MainPos = _mainPlanet.transform.position;

            _attackByRocket = allActions.GetComponent<AttackSomePlanet>();
            _attackByRocket.InitAction();
            if (_attackByRocket==null)
            {
                throw new MyException("attack by rocket = null");
            }

            ScientificCount = 0.0f;/////////////////////////////////////////////
        }
        
        //attack after lost
        //firstly attack neutral
        //attack immediately 
        //firstly attack scientific
        
        public void Enable()
        {
            _isActive = true;
            StartCoroutine(DoSomeAction());
        }

        public void Disable()
        {
            _isActive = false;
        }

        private IEnumerator DoSomeAction()
        {
            while (_isActive)
            {
                float delay = Random.Range(MinDelay, MaxDelay);
                yield return new WaitForSeconds(delay);
                if(_isActive)
                    _attackByRocket.Execute();
            }
        }

        private void AdjustPlanetsList(Planets.Base planet, Planets.Team oldTeam, Planets.Team newTeam)
        {
            AllPlanets[(int) oldTeam].Remove(planet);
            AllPlanets[(int) newTeam].Add(planet);
        }
        
         // public static string LocalIPAddress()
         //         {
         //             IPHostEntry host;
         //             string localIP = "0.0.0.0";
         //             host = Dns.GetHostEntry(Dns.GetHostName());
         //             foreach (IPAddress ip in host.AddressList)
         //             {
         //                 Debug.Log("probable ip "+ip);
         //                 if (ip.AddressFamily == AddressFamily.InterNetwork)
         //                 {
         //                     localIP = ip.ToString();
         //                     break;
         //                 }
         //             }
         //             return localIP;
         //         }
         //
         // public string GetLocalIPv4()
         // {
         //     return Dns.GetHostEntry(Dns.GetHostName())
         //         .AddressList.First(
         //             f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
         //         .ToString();
         // }  
         //
//          public static string GetIP(ADDRESSFAM Addfam)
//          {
//              //Return null if ADDRESSFAM is Ipv6 but Os does not support it
//              if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
//              {
//                  return null;
//              }
//
//              string output = "";
//
//              foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
//              {
// #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//                  NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
//                  NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;
//
//                  if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
// #endif 
//                  {
//                      foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
//                      {
//                          
//                          //IPv4
//                          if (Addfam == ADDRESSFAM.IPv4)
//                          {
//                              if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
//                              {
//                                  output = ip.Address.ToString();
//                              }
//                          }
//
//                          //IPv6
//                          else if (Addfam == ADDRESSFAM.IPv6)
//                          {
//                              if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
//                              {
//                                  output = ip.Address.ToString();
//                              }
//                          }
//                      }
//                  }
//              }
//              return output;
//          }
//          
//          public enum ADDRESSFAM
//          {
//              IPv4, IPv6
//          }
    }
}
