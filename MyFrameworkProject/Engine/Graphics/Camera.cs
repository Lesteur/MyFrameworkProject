using Microsoft.Xna.Framework;

public sealed class Camera
{
    private Vector2 _position;
    private Vector2 _size;
    private float _rotation;

    private Matrix _viewProjection;
    private bool _dirty = true;

    public Camera(int x, int y, int width, int height)
    {
        _position = new Vector2(x, y);
        _size = new Vector2(width, height);
        _rotation = 0f;
        _dirty = true;
    }

    public void SetPosition(int x, int y)
    {
        _position.X = x;
        _position.Y = y;
        _dirty = true;
    }

    public void Move(int dx, int dy)
    {
        _position.X += dx;
        _position.Y += dy;
        _dirty = true;
    }

    public Matrix GetViewProjection()
    {
        if (_dirty)
            Recalculate();

        return _viewProjection;
    }

    private void Recalculate()
    {
        // Projection écran classique (0,0 en haut à gauche)
        var projection = Matrix.CreateOrthographicOffCenter(
            0,
            _size.X,
            _size.Y,
            0,
            0f,
            1f
        );

        // Vue : déplacement inverse de la caméra
        var view = Matrix.CreateTranslation(
            -_position.X,
            -_position.Y,
            0f
        );

        _viewProjection = view * projection;
        _dirty = false;
    }
}