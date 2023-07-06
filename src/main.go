package main

import (
	"log"
	"net/http"
	"os"
	"runtime"
	"strconv"
	"time"

	"github.com/gin-gonic/gin"
)

type HttpResponse struct {
	Status int
	Os     string
	Debug  error
}

func main() {
	r := gin.Default()
	r.LoadHTMLFiles("assets/index.html")
	r.Static("assets/", "./assets")

	r.GET("/crawler", func(c *gin.Context) {
		url := "https://www.githubstatus.com/"
		timeout := 5

		if _, x := os.LookupEnv("HTTP_URL"); x {
			url = os.Getenv("HTTP_URL")
		}

		if _, x := os.LookupEnv("HTTP_TIMEOUT"); x {
			timeout, _ = strconv.Atoi(os.Getenv("HTTP_TIMEOUT"))
		}

		client := &http.Client{
			Timeout: time.Duration(timeout) * time.Second,
		}

		resp, err := client.Get(url)
		if err != nil {
			log.Println(err)
		}

		response := HttpResponse{
			Status: resp.StatusCode,
			Os:     runtime.GOOS,
			Debug:  err,
		}

		c.JSON(resp.StatusCode, gin.H{
			"message": response,
		})
		// Make sure connection is close -.- I'm looking at you conntrack
		defer resp.Body.Close()
	})

	r.GET("/", func(c *gin.Context) {
		image := "Calico-Windows-K8s.png"
		if runtime.GOOS == "linux" {
			image = "Calico-Cat-Tigera-Shirt.png"
		}
		c.HTML(http.StatusOK, "index.html", gin.H{
			"image": image,
		})
	})

	r.Run()

}
