public struct WorldCoords {
    public readonly float worldLeft;
    public readonly float worldRight;
    public readonly float worldBottom;
    public readonly float worldTop;
    public readonly float groundY;
    public float centerX { get { return worldRight - worldLeft; } }

    public WorldCoords(float worldLeft, float worldRight, float worldBottom, float worldTop, float groundY) {
        this.worldLeft = worldLeft;
        this.worldRight = worldRight;
        this.worldBottom = worldBottom;
        this.worldTop = worldTop;
        this.groundY = groundY;
    }
}
