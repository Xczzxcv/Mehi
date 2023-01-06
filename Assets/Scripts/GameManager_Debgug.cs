using UnityEngine;

public partial class GameManager
{
    private string _startPosX;
    private string _startPosY;
    private string _endPosX;
    private string _endPosY;
    private string _lineWidth;

    private void OnGUI()
    {
        return;
        var textFieldStyle = GUI.skin.textField;
        textFieldStyle.fontSize = 20;
        const int startHeight = 30;
        const int startWidth = 20;
        const int sizeHeight = 70;
        const int sizeWidth = 100;
        const int space = 20;
        _startPosX = GUI.TextField(
            new Rect(startWidth, startHeight, sizeWidth, sizeHeight), _startPosX, textFieldStyle);
        _startPosY = GUI.TextField(
            new Rect(startWidth + sizeWidth + space, startHeight, sizeWidth, sizeHeight), _startPosY, textFieldStyle);
        _endPosX = GUI.TextField(
            new Rect(startWidth, startHeight + sizeHeight + space, sizeWidth, sizeHeight), _endPosX, textFieldStyle);
        _endPosY = GUI.TextField(
            new Rect(startWidth + sizeWidth + space, startHeight + sizeHeight + space, sizeWidth, sizeHeight), _endPosY, textFieldStyle);
        _lineWidth = GUI.TextField(
            new Rect(startWidth + (sizeWidth + space) * 2, startHeight + sizeHeight + space, sizeWidth, sizeHeight), _lineWidth, textFieldStyle);

        if (GUI.Button(
            new Rect(startWidth + (sizeWidth * 3 + space * 2 - sizeWidth * 2) / 2, startHeight + sizeHeight * 2 + space, sizeWidth * 2, sizeHeight), 
            "Get intersected tiles"))
        {
            if (!int.TryParse(_startPosX, out var startPosX)
                || !int.TryParse(_startPosY, out var startPosY)
                || !int.TryParse(_endPosX, out var endPosX)
                || !int.TryParse(_endPosY, out var endPosY)
                || !int.TryParse(_lineWidth, out var lineWidth))
            {
                Debug.LogError("Wrong text");
                return;
            }

            var startPos = new Vector2Int(startPosX, startPosY);
            var endPos = new Vector2Int(endPosX, endPosY);
            var intersectedTiles = TilesMathHelper.GetIntersectedTiles(startPos, endPos, lineWidth);
            var intersectedTilesStr = string.Join(", ", intersectedTiles);
            Debug.Log($"Start pos: {startPos},\nend pos: {endPos},\nintersected tiles: {intersectedTilesStr}");
        }
    }
}