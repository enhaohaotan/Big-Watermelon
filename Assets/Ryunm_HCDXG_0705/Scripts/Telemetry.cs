using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using System.Threading;

public class Telemetry : MonoBehaviour
{
    public struct ClickData
    {
        public string startTime;
        public int clickingNumber;
        public Vector2 coordinate;
        public int score;
    }

    /*Telemetry form link*/ //Remember to remove the last part of the link *viewform...*
    private const string GoogleFormBaseUrlA = "https://docs.google.com/forms/d/e/1FAIpQLScSjgKudZYxS0esttY5wiD8pujAI2xD2jwrpfD2YSx7ngfVeQ/";
    private const string GoogleFormBaseUrlB = "https://docs.google.com/forms/d/e/1FAIpQLScaghsCBGxCWt0X6GdoIOevqksI7KAZ6Wth7iwjIWmJMKlbkA/";

    /*Telemtry form variables*/
    private const string _gform_startTime = "entry.648218621";
    private const string _gform_clickingNumber = "entry.701339527";
    private const string _gform_coordinate_x = "entry.1283247003";
    private const string _gform_coordinate_y = "entry.159989255";
    private const string _gform_score = "entry.1618525090";

    private static Guid runID;
    public static IEnumerator SubitGoogleForm(ClickData clickData, string type)
    {
        //These lines make sure that you are not going to have any comma/dot problems. 
        //But you also need to make sure that your spreadsheet settings are UK as well.

        CultureInfo ci = CultureInfo.GetCultureInfo("en-GB");
        Thread.CurrentThread.CurrentCulture = ci;

        string urlGoogleFormResponse;
        if (type == "A")
        {
            urlGoogleFormResponse = GoogleFormBaseUrlA + "formResponse";
        }
        else
        {
            urlGoogleFormResponse = GoogleFormBaseUrlB + "formResponse";
        }

        WWWForm form = new WWWForm();

        form.AddField(_gform_startTime, clickData.startTime);
        form.AddField(_gform_clickingNumber, clickData.clickingNumber);
        form.AddField(_gform_coordinate_x, clickData.coordinate.x.ToString());
        form.AddField(_gform_coordinate_y, clickData.coordinate.y.ToString());
        form.AddField(_gform_score, clickData.score);

        using (UnityWebRequest www = UnityWebRequest.Post(urlGoogleFormResponse, form))
        {
            yield return www.SendWebRequest(); //Decomment this line to send the data!
            
            //You can keep these 2 lines just to be sure that everything is working and there are no errors :)
            yield return null;
            //print("Request sent");
        }
    }

    public static void GenerateNewRunID()
    {
        runID = Guid.NewGuid();
    }

    public static string GUIDToShortString(Guid guid)
    {
        var base64Guid = Convert.ToBase64String(guid.ToByteArray());

        // Replace URL unfriendly characters with better ones
        base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

        // Remove the trailing ==
        return base64Guid.Substring(0, base64Guid.Length - 2);
    }

}
