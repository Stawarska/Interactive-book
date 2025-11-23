using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Book.Animation
{
    [Serializable]
    public class FlipPageAnimation : MonoBehaviour, ISwapPageAnimation
    {
        public Action<FlipMode> OnPageSwapped { get; set; }
        
        public Vector3 EndBottomLeft { get; private set; }
        public Vector3 EndBottomRight { get; private set; }
        
        [SerializeField] private RectTransform bookPanel;
        
        [SerializeField] private bool enableShadowEffect=true;
        
        [SerializeField] private Image clippingPlane;
        [SerializeField] private Image nextPageClip;
        [SerializeField] private Image shadow;
        [SerializeField] private Image shadowLtr;
        [SerializeField] private Image left;
        [SerializeField] private Image leftNext;
        [SerializeField] private Image right;
        [SerializeField] private Image rightNext;
        
        [SerializeField] private float pageFlipTime;
        [SerializeField] private float animationFramesCount;

        private float radius1, radius2;
        private Vector3 sb;
        private Vector3 st;
        private Vector3 c;
        private Vector3 f;
        private bool pageDragging;

        private FlipMode mode;
    
        private Coroutine currentCoroutine;
        
        private RectTransform currentSpawnedPage;
        private Page nextPage;
        

        private void Awake()
        {
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
            CalcCurlCriticalPoints();

            var rect = bookPanel.rect;
            float pageWidth = rect.width / 2.0f;
            float pageHeight = rect.height;
            nextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageHeight * 2);
            clippingPlane.rectTransform.sizeDelta =
                new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

            //hypotenous (diagonal) page length
            float hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
            float shadowPageHeight = pageWidth / 2 + hyp;

            shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadow.rectTransform.pivot = new Vector2(1, pageWidth / 2 / shadowPageHeight);

            shadowLtr.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
            shadowLtr.rectTransform.pivot = new Vector2(0, pageWidth / 2 / shadowPageHeight);
        }

        private void CalcCurlCriticalPoints()
        {
            var rect = bookPanel.rect;
            sb = new Vector3(0, -rect.height / 2);
            EndBottomRight = new Vector3(rect.width / 2, -rect.height / 2);
            EndBottomLeft = new Vector3(-rect.width / 2, -rect.height / 2);
            st = new Vector3(0, rect.height / 2);
            radius1 = Vector2.Distance(sb, EndBottomRight);
            var rect1 = bookPanel.rect;
            float pageWidth = rect1.width / 2.0f;
            float pageHeight = rect1.height;
            radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        }

        private void UpdateBookLtrToPoint(Vector3 followLocation)
        {
            mode = FlipMode.LeftToRight;
            f = followLocation;
            Transform transform1;
            (transform1 = shadowLtr.transform).SetParent(clippingPlane.transform, true);
            transform1.localPosition = new Vector3(0, 0, 0);
            transform1.localEulerAngles = new Vector3(0, 0, 0);
            left.transform.SetParent(clippingPlane.transform, true);

            Transform transform2;
            (transform2 = right.transform).SetParent(bookPanel.transform, true);
            transform2.localEulerAngles = Vector3.zero;
            leftNext.transform.SetParent(bookPanel.transform, true);

            c = Calc_C_Position(followLocation);
            float clipAngle = CalcClipAngle(c, EndBottomLeft, out var t1);
            //0 < T0_T1_Angle < 180
            clipAngle = (clipAngle + 180) % 180;

            clippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
            clippingPlane.transform.position = bookPanel.TransformPoint(t1);

            //page position and angle
            left.transform.position = bookPanel.TransformPoint(c);
            float cT1Dy = t1.y - c.y;
            float cT1Dx = t1.x - c.x;
            float cT1Angle = Mathf.Atan2(cT1Dy, cT1Dx) * Mathf.Rad2Deg;
            left.transform.localEulerAngles = new Vector3(0, 0, cT1Angle - 90 - clipAngle);

            nextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
            Transform transform3;
            (transform3 = nextPageClip.transform).position = bookPanel.TransformPoint(t1);
            leftNext.transform.SetParent(transform3, true);
            right.transform.SetParent(clippingPlane.transform, true);
            right.transform.SetAsFirstSibling();

            shadowLtr.rectTransform.SetParent(left.rectTransform, true);
        }

        private void UpdateBookRtlToPoint(Vector3 followLocation)
        {
            mode = FlipMode.RightToLeft;
            f = followLocation;
            Transform transform1;
            (transform1 = shadow.transform).SetParent(clippingPlane.transform, true);
            transform1.localPosition = Vector3.zero;
            transform1.localEulerAngles = Vector3.zero;
            right.transform.SetParent(clippingPlane.transform, true);

            Transform transform2;
            (transform2 = left.transform).SetParent(bookPanel.transform, true);
            transform2.localEulerAngles = Vector3.zero;
            rightNext.transform.SetParent(bookPanel.transform, true);
            c = Calc_C_Position(followLocation);
            float clipAngle = CalcClipAngle(c, EndBottomRight, out var t1);
            if (clipAngle > -90) clipAngle += 180;

            clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
            clippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
            clippingPlane.transform.position = bookPanel.TransformPoint(t1);

            //page position and angle
            right.transform.position = bookPanel.TransformPoint(c);
            float cT1Dy = t1.y - c.y;
            float cT1Dx = t1.x - c.x;
            float cT1Angle = Mathf.Atan2(cT1Dy, cT1Dx) * Mathf.Rad2Deg;
            right.transform.localEulerAngles = new Vector3(0, 0, cT1Angle - (clipAngle + 90));

            nextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
            Transform transform3;
            (transform3 = nextPageClip.transform).position = bookPanel.TransformPoint(t1);
            rightNext.transform.SetParent(transform3, true);
            left.transform.SetParent(clippingPlane.transform, true);
            left.transform.SetAsFirstSibling();

            shadow.rectTransform.SetParent(right.rectTransform, true);
        }

        private float CalcClipAngle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
        {
            var t0 = (c + bookCorner) / 2;
            float t0CornerDy = bookCorner.y - t0.y;
            float t0CornerDx = bookCorner.x - t0.x;
            float t0CornerAngle = Mathf.Atan2(t0CornerDy, t0CornerDx);

            float t1X = t0.x - t0CornerDy * Mathf.Tan(t0CornerAngle);
            t1X = NormalizeT1X(t1X, bookCorner, sb);
            t1 = new Vector3(t1X, sb.y, 0);

            //clipping plane angle=T0_T1_Angle
            float t0T1Dy = t1.y - t0.y;
            float t0T1Dx = t1.x - t0.x;
            float t0T1Angle = Mathf.Atan2(t0T1Dy, t0T1Dx) * Mathf.Rad2Deg;
            return t0T1Angle;
        }

        private float NormalizeT1X(float t1, Vector3 corner, Vector3 sb)
        {
            if (t1 > sb.x && sb.x > corner.x)
                return sb.x;
            if (t1 < sb.x && sb.x < corner.x)
                return sb.x;
            return t1;
        }

        private Vector3 Calc_C_Position(Vector3 followLocation)
        {
            Vector3 c;
            f = followLocation;
            float fSbDy = f.y - sb.y;
            float fSbDx = f.x - sb.x;
            float fSbAngle = Mathf.Atan2(fSbDy, fSbDx);
            var r1 = new Vector3(radius1 * Mathf.Cos(fSbAngle), radius1 * Mathf.Sin(fSbAngle), 0) + sb;

            float fSbDistance = Vector2.Distance(f, sb);
            c = fSbDistance < radius1 ? f : r1;
            float fStDy = c.y - st.y;
            float fStDx = c.x - st.x;
            float fStAngle = Mathf.Atan2(fStDy, fStDx);
            var r2 = new Vector3(radius2 * Mathf.Cos(fStAngle),
                radius2 * Mathf.Sin(fStAngle), 0) + st;
            float cStDistance = Vector2.Distance(c, st);
            if (cStDistance > radius2)
                c = r2;
            return c;
        }

        private void DragRightPageToPoint(Vector3 point)
        {
            pageDragging = true;
            mode = FlipMode.RightToLeft;
            f = point;

            nextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
            clippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

            left.gameObject.SetActive(true);
            left.rectTransform.pivot = new Vector2(0, 0);
            var transform1 = left.transform;
            transform1.position = rightNext.transform.position;
            transform1.eulerAngles = new Vector3(0, 0, 0);
            left.transform.SetAsFirstSibling();

            right.gameObject.SetActive(true);
            var transform2 = right.transform;
            transform2.position = rightNext.transform.position;
            transform2.eulerAngles = new Vector3(0, 0, 0);

            leftNext.transform.SetAsFirstSibling();
            if (enableShadowEffect) shadow.gameObject.SetActive(true);
            UpdateBookRtlToPoint(f);


            // if (currentPage != null)
            //     left.sprite = currentPage.ImagePagePrefab.SpritePage;
            //
            // right.sprite = targetPage.TextPagePrefab.SpritePage;
            // rightNext.sprite = targetPage.ImagePagePrefab.SpritePage;
        }

        private void DragLeftPageToPoint(Vector3 point)
        {
            pageDragging = true;
            mode = FlipMode.LeftToRight;
            f = point;

            nextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
            clippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

            right.gameObject.SetActive(true);
            var transform1 = right.transform;
            var position = leftNext.transform.position;
            transform1.position = position;
            transform1.eulerAngles = new Vector3(0, 0, 0);
            right.transform.SetAsFirstSibling();

            left.gameObject.SetActive(true);
            left.rectTransform.pivot = new Vector2(1, 0);
            var transform2 = left.transform;
            transform2.position = position;
            transform2.eulerAngles = new Vector3(0, 0, 0);

            rightNext.transform.SetAsFirstSibling();
            if (enableShadowEffect) shadowLtr.gameObject.SetActive(true);
            UpdateBookLtrToPoint(f);

            // right.sprite = currentPage.TextPagePrefab.SpritePage;
            // left.sprite = targetPage.ImagePagePrefab.SpritePage;
            // leftNext.sprite = targetPage.TextPagePrefab.SpritePage;
        }


        private void TweenForward() => currentCoroutine = StartCoroutine(mode == FlipMode.RightToLeft
            ? TweenTo(EndBottomLeft, 0.15f, Flip)
            : TweenTo(EndBottomRight, 0.15f, Flip));

        private void Flip()
        {
            leftNext.transform.SetParent(bookPanel.transform, true);
            leftNext.transform.SetAsFirstSibling();
            left.transform.SetParent(bookPanel.transform, true);
            //leftNext.transform.SetParent(bookPanel.transform, true);
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
            right.transform.SetParent(bookPanel.transform, true);
            rightNext.transform.SetParent(bookPanel.transform, true);
            shadow.gameObject.SetActive(false);
            shadowLtr.gameObject.SetActive(false);

            currentSpawnedPage = Instantiate(nextPage, bookPanel).GetComponent<RectTransform>();
            currentSpawnedPage.anchorMin = Vector2.zero;
            currentSpawnedPage.anchorMax = Vector2.one;
            currentSpawnedPage.offsetMax = Vector2.zero;
            currentSpawnedPage.offsetMin = Vector2.zero;

            // leftNext.sprite = targetPage.TextPagePrefab.SpritePage;
            // rightNext.sprite = targetPage.ImagePagePrefab.SpritePage;
        }

        private void TweenBack()
        {
            if (mode == FlipMode.RightToLeft)
            {
                currentCoroutine = StartCoroutine(TweenTo(EndBottomRight, 0.15f,
                    () =>
                    {
                        rightNext.transform.SetParent(bookPanel.transform);
                        right.transform.SetParent(bookPanel.transform);

                        left.gameObject.SetActive(false);
                        right.gameObject.SetActive(false);
                        pageDragging = false;
                    }
                ));
            }
            else
            {
                currentCoroutine = StartCoroutine(TweenTo(EndBottomLeft, 0.15f,
                    () =>
                    {

                        leftNext.transform.SetParent(bookPanel.transform);
                        left.transform.SetParent(bookPanel.transform);

                        left.gameObject.SetActive(false);
                        right.gameObject.SetActive(false);
                        pageDragging = false;
                    }
                ));
            }
        }

        private IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
        {
            int steps = (int)(duration / 0.025f);
            var displacement = (to - f) / steps;
            for (int i = 0; i < steps - 1; i++)
            {
                if (mode == FlipMode.RightToLeft)
                    UpdateBookRtlToPoint(f + displacement);
                else
                    UpdateBookLtrToPoint(f + displacement);

                yield return new WaitForSeconds(0.025f);
            }

            onFinish?.Invoke();
        }

        private IEnumerator FlipRTL(float bottomMiddlePoint, float xl, float h, float frameTime, float dx)
        {
            float x = bottomMiddlePoint + xl;
            float y = -h / (xl * xl) * (x - bottomMiddlePoint) * (x - bottomMiddlePoint);

            DragRightPageToPoint(new Vector3(x, y, 0));
            for (int i = 0; i < animationFramesCount; i++)
            {
                y = -h / (xl * xl) * (x - bottomMiddlePoint) * (x - bottomMiddlePoint);
                UpdateBookRtlToPoint(new Vector3(x, y, 0));
                yield return new WaitForSeconds(frameTime);
                x -= dx;
            }

            ReleasePage();
        }

        private IEnumerator FlipLTR(float bottomMiddlePoint, float xl, float h, float frameTime, float dx)
        {
            float x = bottomMiddlePoint - xl;
            float y = -h / (xl * xl) * (x - bottomMiddlePoint) * (x - bottomMiddlePoint);

            DragLeftPageToPoint(new Vector3(x, y, 0));
            for (int i = 0; i < animationFramesCount; i++)
            {
                y = -h / (xl * xl) * (x - bottomMiddlePoint) * (x - bottomMiddlePoint);
                UpdateBookLtrToPoint(new Vector3(x, y, 0));
                yield return new WaitForSeconds(frameTime);
                x += dx;
            }

            ReleasePage();
        }

        private void ReleasePage()
        {
            if (!pageDragging)
                return;
            pageDragging = false;
            float distanceToLeft = Vector2.Distance(c, EndBottomLeft);
            float distanceToRight = Vector2.Distance(c, EndBottomRight);
            if (distanceToRight < distanceToLeft && mode == FlipMode.RightToLeft)
                TweenBack();
            else if (distanceToRight > distanceToLeft && mode == FlipMode.LeftToRight)
                TweenBack();
            else
                TweenForward();
        }

        public void FlipRightPage()
        {
            if (pageDragging)
                return;

            float frameTime = pageFlipTime / animationFramesCount;
            float bottomMiddlePoint = (EndBottomRight.x + EndBottomLeft.x) / 2;
            float xl = (EndBottomRight.x - EndBottomLeft.x) / 2 * 0.9f;
            float h = Mathf.Abs(EndBottomRight.y) * 0.9f;
            float dx = xl * 2 / animationFramesCount;

            StartCoroutine(FlipRTL(bottomMiddlePoint, xl, h, frameTime, dx));
        }

        public void FlipLeftPage()
        {
            if (pageDragging)
                return;

            float frameTime = pageFlipTime / animationFramesCount;
            float bottomMiddlePoint = (EndBottomRight.x + EndBottomLeft.x) / 2;
            float xl = (EndBottomRight.x - EndBottomLeft.x) / 2 * 0.9f;
            float h = Mathf.Abs(EndBottomRight.y) * 0.9f;
            float dx = xl * 2 / animationFramesCount;
            StartCoroutine(FlipLTR(bottomMiddlePoint, xl, h, frameTime, dx));
        }

        public void FlipPage(Page page)
        {
            //TODO: dodać implementacje
            
            if(currentSpawnedPage)
                Destroy(currentSpawnedPage.gameObject);
      
            nextPage = page;
            FlipLeftPage();
        }
    }
}