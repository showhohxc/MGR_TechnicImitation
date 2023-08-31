# MGR_TechnicImitation
Metal Gear Rising Imitation Project using Unity

> Youtube

[![Video Label](http://img.youtube.com/vi/dRMa3uiUSvA/0.jpg)](https://youtu.be/dRMa3uiUSvA)
▲ 위의 Metal Gear Rising의 Cutting 모드를 모작

[![Video Label](http://img.youtube.com/vi/Si52V8eWYmU/0.jpg)](https://youtu.be/Si52V8eWYmU)
▲ Unity 2020.3.17f1을 이용해 구현

### 사용 라이브러리
https://github.com/DavidArayan/ezy-slice

## 비고
● 일반적인 Mesh Renderer 같은 경우 사용 Vertex가 적어 원활한 프레임을 유지할수 있지만 Skinned Renderer를 적용시 Vertex 양이 증가함에 따라 Slice 시 프레임이 떨어지는 현상이 발생
● 고정되어 있는 물체에 한해서는 최적이지만 플레이와 같은 캐릭터 형식의 Renderer 일시 신체의 파트별로 Collider를 적용하여 Cut 모드일시 적용되게끔 변경하는게 베스트이지 않을까 생각한다.
