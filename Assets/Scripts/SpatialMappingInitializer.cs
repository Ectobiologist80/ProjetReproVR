using Meta.XR.SpatialAwareness; // Namespace spécifique à Meta SDK (peut varier selon la version, vérifier dans Meta Docs)
using UnityEngine;
using UnityEngine.XR.ARFoundation; // Pour le Spatial Awareness standard

public class SpatialMappingInitializer : MonoBehaviour
{
    // Référence au système de conscience spatiale
    private SpatialAwarenessSystem spatialAwarenessSystem;

    void Start()
    {
        // 1. Initialisation du système Spatial Awareness
        // Le MRUK fournit souvent un composant dédié, ou on utilise le standard ARFoundation
        spatialAwarenessSystem = FindObjectOfType<SpatialAwarenessSystem>();

        if (spatialAwarenessSystem == null)
        {
            Debug.LogError("SpatialAwarenessSystem non trouvé ! Vérifiez les packages Meta XR.");
            return;
        }

        // 2. Configuration du mode de génération de maillage
        // Vous voulez le maillage de la pièce (Scene Mesh)
        spatialAwarenessSystem.MeshGenerationMode = SpatialAwarenessMeshGenerationMode.Continuous;

        // 3. Activation de la détection
        spatialAwarenessSystem.OnMeshDataChanged += OnMeshDataChanged;

        // 4. Démarrage de la génération
        // Le SDK commence à scannner la pièce
        spatialAwarenessSystem.StartMeshGeneration();

        Debug.Log("Spatial Mapping initialisé. La pièce est en cours de scan.");
    }

    // Callback appelé quand un nouveau maillage est généré
    private void OnMeshDataChanged(SpatialAwarenessSystem sender, SpatialAwarenessSystem.MeshData meshData)
    {
        // C'est ici que vous pouvez traiter le maillage généré
        // Par exemple : appliquer des textures, détecter des surfaces, etc.
        if (meshData.Mesh != null)
        {
            Debug.Log($"Maillage généré : {meshData.Mesh.name}");
            // Logique de substitution ici (voir étape 3)
        }
    }

    void OnDestroy()
    {
        // Nettoyage
        if (spatialAwarenessSystem != null)
        {
            spatialAwarenessSystem.OnMeshDataChanged -= OnMeshDataChanged;
            spatialAwarenessSystem.StopMeshGeneration();
        }
    }
}