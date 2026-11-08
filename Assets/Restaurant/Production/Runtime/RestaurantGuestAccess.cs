using TMPro;
using UnityEngine;
using RestaurantPreview;

namespace IdleRestaurant
{
    public sealed partial class RestaurantWorld
    {
        // Shared geometry endpoints keep visible doorways and simulation paths aligned.
        public static readonly Vector3 EntranceSpawn = new Vector3(-4.15f, 0, -5.85f);
        public static readonly Vector3 ExitThreshold = new Vector3(4.4f, 0, -5.2f);
        public static readonly Vector3 ExitDespawn = new Vector3(4.4f, 0, -5.85f);
        public static readonly Vector3 TrayReturnPosition = new Vector3(4.6f, 0, -4.65f);
        public static readonly Vector3 TrayReturnApproach = new Vector3(3.85f, 0, -4.65f);
        public static readonly Vector3 TrayReturnClearance = new Vector3(4.1f, 0, -5.2f);
        public const int QueueCapacity = 10;

        void BuildGuestAccess()
        {
            var access = Models.Group("Guest entrance and exit", room, Vector3.zero);
            Models.Box(access, new Vector3(0, -.04f, -5.65f), new Vector3(10.7f, .08f, .95f), "#B9B8A3", .04f);
            Doorway(access, new Vector3(EntranceSpawn.x, 0, -5.2f), "ENTER", "#365C49", true);
            Doorway(access, ExitThreshold, "EXIT", "#A7523C", false);
            // A short, bounded queue remains inside the restaurant rather than extending through the scenery.
            for (int i = 0; i < 5; i++)
                Models.Box(access, new Vector3(-4.15f, .018f, -4.25f + i * .65f),
                    new Vector3(.42f, .025f, .06f), "#83A487", .01f);
        }

        void Doorway(Transform parent, Vector3 position, string label, string color, bool entering)
        {
            var door = Models.Group(label + " doorway", parent, position);
            for (int side = -1; side <= 1; side += 2)
            {
                var post = Models.Box(door, new Vector3(side * .62f, .72f, 0),
                    new Vector3(.14f, 1.44f, .22f), color, .03f);
                var block = post.gameObject.AddComponent<BoxCollider>();
                block.size = new Vector3(.14f, 1.44f, .22f); // Rounded blocks bake their dimensions into the mesh.
            }
            Models.Box(door, new Vector3(0, 1.48f, 0), new Vector3(1.42f, .3f, .28f), color, .06f);
            Models.Box(door, new Vector3(0, .025f, 0), new Vector3(1.1f, .04f, .8f), "#E8D7AA", .03f);
            var plaque = Models.Box(door, new Vector3(0, 1.72f, 0), new Vector3(1.45f, .45f, .12f), "#304D40", .05f);
            if (ViewCamera) plaque.rotation = ViewCamera.transform.rotation;
            AccessLabel(door, label, new Vector3(0, 1.73f, -.10f));
            float direction = entering ? 1 : -1;
            float z = entering ? -.3f : .3f;
            Models.Tube(door, new[] { new Vector3(0, .06f, z - direction * .17f),
                new Vector3(0, .06f, z + direction * .17f) }, .045f, "#F7C644", 6);
            Models.Tube(door, new[] { new Vector3(-.14f, .06f, z),
                new Vector3(0, .06f, z + direction * .17f), new Vector3(.14f, .06f, z) }, .045f, "#F7C644", 6);
        }

        void AccessLabel(Transform parent, string value, Vector3 position)
        {
            var go = new GameObject(value + " sign", typeof(TextMeshPro), typeof(RestaurantActorMarker));
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            if (ViewCamera) { go.transform.rotation = ViewCamera.transform.rotation; go.transform.position -= ViewCamera.transform.forward * .18f; }
            var text = go.GetComponent<TextMeshPro>();
            text.text = value;
            text.fontSize = 2.8f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Models.C("#FFF8EC");
            text.rectTransform.sizeDelta = new Vector2(1.35f, .42f);
            text.textWrappingMode = TextWrappingModes.NoWrap;
        }
    }
}
