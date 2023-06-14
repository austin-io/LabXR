using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour {
    
    [SerializeField] int _flockSize = 10;
    [SerializeField] float _boidSpeed = 10, _maxAlignment = 0.1f, _maxCohesion = 0.1f, _maxSeperation = 0.1f;
    [SerializeField] float _localRadius = 1;

    [SerializeField] GameObject _boidPrefab;
    List<Boid> _boids = new List<Boid>();

    // Start is called before the first frame update
    void Start() {
        for(int i = 0; i < _flockSize; i++){
            GameObject go = Instantiate(_boidPrefab, transform.position + new Vector3(Random.Range(-2, 2),Random.Range(-2, 2),Random.Range(-2, 2)), Quaternion.Euler(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)));
            Boid b = go.GetComponent<Boid>();
            b.Init(b.transform.forward, _boidSpeed);
            _boids.Add(b);
        }
    }

    void alignment(){
        for(int i = 0; i < _boids.Count; i++){
            Vector3 averageVelocity = Vector3.zero;
            int numberOfNeighbours = 0;

            for(int j = 0; j < _boids.Count; j++){
                if(i == j) continue;
                if((_boids[i].transform.position - _boids[j].transform.position).sqrMagnitude > _localRadius) continue;
                numberOfNeighbours++;
                averageVelocity += _boids[j].GetVelocity();
            }

            if(numberOfNeighbours == 0) continue;
            averageVelocity /= numberOfNeighbours;
            averageVelocity = Vector3.ClampMagnitude(averageVelocity, _maxAlignment);
            Vector3 force = averageVelocity - _boids[i].GetVelocity();
            force = Vector3.ClampMagnitude(force, _maxAlignment);
            _boids[i].AddForce(force);

        }
    }

    void cohesion(){
        for(int i = 0; i < _boids.Count; i++){
            Vector3 averagePosition = Vector3.zero;
            int numberOfNeighbours = 0;

            for(int j = 0; j < _boids.Count; j++){
                if(i == j) continue;
                if((_boids[i].transform.position - _boids[j].transform.position).sqrMagnitude > _localRadius) continue;
                numberOfNeighbours++;
                averagePosition += _boids[j].transform.position;
            }

            if(numberOfNeighbours == 0) continue;
            averagePosition /= numberOfNeighbours;

            Vector3 force = Vector3.ClampMagnitude(averagePosition - _boids[i].transform.position, _maxCohesion) - _boids[i].GetVelocity();
            force = Vector3.ClampMagnitude(force, _maxCohesion);
            _boids[i].AddForce(force);

        }
    }

    void seperation(){
        for(int i = 0; i < _boids.Count; i++){
            Vector3 targetVelocity = Vector3.zero;
            int numberOfNeighbours = 0;

            for(int j = 0; j < _boids.Count; j++){
                if(i == j) continue;
                if((_boids[i].transform.position - _boids[j].transform.position).sqrMagnitude > _localRadius) continue;
                if((_boids[i].transform.position - _boids[j].transform.position).sqrMagnitude == 0) {
                    _boids[i].transform.position += Vector3.forward * 0.1f;
                }
                numberOfNeighbours++;
                Vector3 offset = _boids[i].transform.position - _boids[j].transform.position;
                offset /= offset.sqrMagnitude;
                targetVelocity += offset;
            }

            if(numberOfNeighbours == 0) continue;
            //targetVelocity /= numberOfNeighbours;

            targetVelocity = Vector3.ClampMagnitude(targetVelocity, _maxSeperation);

            Vector3 force = (targetVelocity - _boids[i].GetVelocity());
            
            force = Vector3.ClampMagnitude(force, _maxSeperation);
            _boids[i].AddForce(force);

        }
    }

    // Update is called once per frame
    void Update() {
        alignment();
        cohesion();
        seperation();
    }
}
