# PlayGound
Youth Sports Platform

# Windows에서 Redis 실행하기

## 1. 실행 파일로 Redis 구동하기

이 방법은 별도의 설치 과정 없이 다운로드한 파일을 실행만 하면 됩니다.

### 설치 단계

1. **Redis 비공식 릴리스 다운로드**
	- GitHub의 **redis-server** 저장소에서 최신 **릴리스(.zip)** 파일을 다운로드합니다
	- 이 릴리스는 Microsoft에서 공식적으로 지원하지 않지만, 개발용으로는 충분히 안정적입니다
	- https://github.com/redis-windows/redis-windows

2. **파일 압축 해제**
	- 다운로드한 ZIP 파일의 압축을 풀어 원하는 디렉터리(예: `C:\Redis`)에 저장합니다

3. **Redis 서버 실행**
	- 압축을 해제한 폴더에서 `redis-server.exe` 파일을 실행합니다
	- 명령 프롬프트 창이 뜨면서 Redis 서버가 구동됩니다

4. **클라이언트 접속**
	- 다른 명령 프롬프트 창을 열고, 같은 폴더의 `redis-cli.exe`를 실행하면 Redis 서버에 접속할 수 있습니다

## 2. 프로젝트에 Redis 실행 파일 포함시키기

### 구성 방법

1. **프로젝트 폴더에 복사**
	- 위에서 다운로드한 `redis-server.exe`와 `redis-cli.exe` 파일을 **프로젝트 폴더**나 별도의 `lib` 폴더에 복사합니다

2. **프로젝트 시작 시 Redis 서버 자동 실행**
	- 애플리케이션 시작 시 Redis 서버를 자동으로 구동하도록 코드를 추가할 수 있습니다
	- 예를 들어, C# .NET 환경에서는 `System.Diagnostics.Process`를 사용하여 실행 파일을 호출할 수 있습니다
