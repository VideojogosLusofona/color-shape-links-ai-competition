/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;
using UnityEngine.UI;

// This script updates the "who is thinking" label when an AI is thinking
public class WhoIsThinking : MonoBehaviour
{
    // Reference to the session data
    private IMatchDataProvider sessionData;
    // The UI text label to update
    private Text label;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        sessionData = GetComponentInParent<IMatchDataProvider>();
        label = GetComponent<Text>();
        label.text = sessionData.CurrentPlayer.PlayerName;
    }

    // Update is called once per frame
    private void Update()
    {
        label.text = $"{sessionData.CurrentPlayer.PlayerName} is thinking...";
    }
}
