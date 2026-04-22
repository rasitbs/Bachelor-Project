using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SnapInteractableVisuals : MonoBehaviour
{
    [SerializeField] private SnapInteractable snapInteractable;
    [SerializeField] private Material hoverMaterial;

    private GameObject currentInteractorGameObject;
    private SnapInteractor currentInteractor;

    private void OnEnable()
    {
        snapInteractable.WhenInteractorAdded.Action += WhenInteractorAdded_Action;
        snapInteractable.WhenSelectingInteractorViewAdded += SnapInteractable_WhenSelectingInteractorViewAdded;
        snapInteractable.WhenInteractorViewRemoved += SnapInteractable_WhenInteractorViewRemoved;
        snapInteractable.WhenInteractorViewAdded += SnapInteractable_WhenInteractorViewAdded;
    }

    private void WhenInteractorAdded_Action(SnapInteractor obj)
    {
        if (currentInteractor == null)
            currentInteractor = obj;
        else if (currentInteractor != obj)
        {
            currentInteractor = obj;
            var tempGP = currentInteractorGameObject;
            Destroy(tempGP);
            currentInteractorGameObject = null;
        }
        else
            return;

        SetupGhostModel(obj);
    }

    private void SnapInteractable_WhenSelectingInteractorViewAdded(IInteractorView obj)
    {
        currentInteractorGameObject?.SetActive(false);
    }

    private void SnapInteractable_WhenInteractorViewAdded(IInteractorView obj)
    {
        currentInteractorGameObject?.SetActive(true);
    }

    private void SnapInteractable_WhenInteractorViewRemoved(IInteractorView obj)
    {
        currentInteractorGameObject.SetActive(false);
    }

    private void SetupGhostModel(SnapInteractor interactor)
    {
        Transform originalRoot = interactor.transform.parent;
        if (originalRoot == null) return;


        currentInteractorGameObject = new GameObject("GhostVisual_" + originalRoot.name);
        currentInteractorGameObject.transform.SetParent(transform, false);


        BoxCollider targetCollider = GetComponent<BoxCollider>();
        if (targetCollider != null)
        {
            currentInteractorGameObject.transform.localPosition = targetCollider.center;
        }
        else
        {
            currentInteractorGameObject.transform.localPosition = Vector3.zero;
        }

        currentInteractorGameObject.transform.localRotation = Quaternion.identity;


        Vector3 parentScale = transform.lossyScale;
        currentInteractorGameObject.transform.localScale = new Vector3(
            originalRoot.lossyScale.x / parentScale.x,
            originalRoot.lossyScale.y / parentScale.y,
            originalRoot.lossyScale.z / parentScale.z
        );

        var allMeshes = originalRoot.GetComponentsInChildren<MeshFilter>();

        foreach (var item in allMeshes)
        {
            GameObject visualPart = new GameObject(item.name + "_Ghost");
            visualPart.transform.SetParent(currentInteractorGameObject.transform, false);


            Vector3 relativePos = interactor.transform.InverseTransformPoint(item.transform.position);
            Quaternion relativeRot = Quaternion.Inverse(interactor.transform.rotation) * item.transform.rotation;

            visualPart.transform.localPosition = relativePos;
            visualPart.transform.localRotation = relativeRot;

            Vector3 worldScale = item.transform.lossyScale;
            visualPart.transform.localScale = new Vector3(
                worldScale.x / originalRoot.lossyScale.x,
                worldScale.y / originalRoot.lossyScale.y,
                worldScale.z / originalRoot.lossyScale.z
            );

            visualPart.AddComponent<MeshFilter>().sharedMesh = item.sharedMesh;
            visualPart.AddComponent<MeshRenderer>().sharedMaterial = hoverMaterial;
        }
    }
}
