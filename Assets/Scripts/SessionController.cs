/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

public class SessionController : MonoBehaviour
{

    private GameController gameController = null;

    private void Awake()
    {
        gameController =
            GameObject.Find("Game")?.GetComponent<GameController>();
    }

    private void OnGUI()
    {
        if (gameController.IsOver)
        {
            GUI.Window(0,
                new Rect(
                    Screen.width / 2 - 100,
                    Screen.height / 2 - 50, 200, 100),
                DrawGameOverWindow,
                "Game Over!");
        }
    }

    // Draw window contents
    private void DrawGameOverWindow(int id)
    {
        // Is this the correct window?
        if (id == 0)
        {
            // Draw OK button
            if (GUI.Button(new Rect(50, 40, 100, 30), "OK"))
            {
                // If button is clicked, exit
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }
}
